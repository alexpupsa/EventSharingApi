using EventSharingApi.Models;
using EventSharingApi.Repository;
using Microsoft.AspNetCore.Mvc;

namespace EventSharingApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EventController : ControllerBase
    {
        private readonly IEventRepository _eventRepository;
        private readonly IEventAttendeeRepository _eventAttendeeRepository;

        public EventController(IEventRepository eventRepository, IEventAttendeeRepository eventAttendeeRepository)
        {
            _eventRepository = eventRepository;
            _eventAttendeeRepository = eventAttendeeRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await _eventRepository.GetAll());
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] EventModel eventModel)
        {
            eventModel.State = EventState.Published;
            var result = await _eventRepository.Add(eventModel);
            if (result)
            {
                return NoContent();
            }
            return BadRequest();
        }

        [HttpGet("register/{userId}/to-event/{eventId}")]
        public async Task<IActionResult> RegisterUserToEvent([FromRoute] int userId, [FromRoute] int eventId)
        {
            var targetEvent = await _eventRepository.Get(eventId);
            if (targetEvent == null || targetEvent.State == EventState.Cancelled || targetEvent.State == EventState.Finished)
            {
                return BadRequest();
            }

            var result = await _eventAttendeeRepository.Add(eventId, userId, targetEvent.Quota);
            if (result)
            {
                return NoContent();
            }
            return BadRequest();
        }
    }
}