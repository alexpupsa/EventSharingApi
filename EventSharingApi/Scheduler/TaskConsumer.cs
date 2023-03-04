using EventSharingApi.Repository;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace EventSharingApi.Scheduler
{
    public class TaskConsumer
    {
        private readonly ConnectionFactory _factory;
        private readonly string _queueName;
        private readonly IEventRepository _eventRepository;
        private readonly IEventAttendeeRepository _eventAttendeeRepository;

        public TaskConsumer(ConnectionFactory factory, string queueName, IEventRepository eventRepository, IEventAttendeeRepository eventAttendeeRepository)
        {
            _factory = factory;
            _queueName = queueName;
            _eventRepository = eventRepository;
            _eventAttendeeRepository = eventAttendeeRepository;
        }

        public void Consume()
        {
            using (var connection = _factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: _queueName,
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += async (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine($"Message received: {message}");

                    // sending notifications
                    var events = await _eventRepository.GetAllEventsThatStartInMinutes(15);
                    if (events.Any())
                    {
                        foreach (var e in events)
                        {
                            var participants = await _eventAttendeeRepository.GetParticipantIds(e.Id);
                            foreach (var participant in participants)
                            {
                                Console.WriteLine($"Event {e.Title} is starting in 15 minutes for participant {participant}");
                            }
                        }
                    }

                    //updating events that finished
                    var finishedEventIds = await _eventRepository.GetAllEventIdsThatShouldBeMarkedAsFinished();
                    if (finishedEventIds.Any())
                    {
                        await _eventRepository.MarkEventsAsFinished(finishedEventIds);
                    }

                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                };

                channel.BasicConsume(queue: _queueName,
                                     autoAck: false,
                                     consumer: consumer);

                Console.WriteLine($"Listening for messages on {_queueName}. Press [Enter] to exit.");
                Console.ReadLine();
            }
        }
    }

}
