using Quartz;
using RabbitMQ.Client;
using System.Text;

namespace EventSharingApi.Scheduler
{
    public class ScheduledTask : IJob
    {
        private readonly ConnectionFactory _factory;
        private readonly string _queueName;

        public ScheduledTask(ConnectionFactory factory, string queueName)
        {
            _factory = factory;
            _queueName = queueName;
        }

        public Task Execute(IJobExecutionContext context)
        {
            // Publish a message to RabbitMQ
            using (var connection = _factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: _queueName,
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var message = "Check events";
                var body = Encoding.UTF8.GetBytes(message);
                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;

                channel.BasicPublish(exchange: "",
                                     routingKey: _queueName,
                                     basicProperties: properties,
                                     body: body);

                Console.WriteLine($"Task executed at {DateTime.Now}, message published to {_queueName}");
            }

            return Task.CompletedTask;
        }
    }
}
