using EventSharingApi.Context;
using EventSharingApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventSharingApi.Repository
{
    public class EventAttendeeRepository : IEventAttendeeRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly object _lock;

        public EventAttendeeRepository(AppDbContext appDbContext)
        {
            _dbContext = appDbContext;
            _lock = new object();
        }

        public async Task<bool> Add(int eventId, int userId, int quota)
        {
            var userAtEvent = await _dbContext.EventAttendees.AnyAsync(x => x.EventId == eventId && x.UserId == userId);
            if (userAtEvent)
            {
                return false;
            }

            lock (_lock)
            {
                var numberOfParticipants = _dbContext.EventAttendees.Count(x => x.EventId == eventId);
                if (numberOfParticipants >= quota)
                {
                    return false;
                }
                _dbContext.Add(new EventAttendee { EventId = eventId, UserId = userId });
                var result = _dbContext.SaveChanges();
                return result > 0;
            }
        }

        public async Task<int> GetEventNumberOfParticipants(int eventId)
        {
            return await _dbContext.EventAttendees.CountAsync(x => x.EventId == eventId);
        }

        public async Task<List<int>> GetParticipantIds(int eventId)
        {
            return await _dbContext.EventAttendees.Where(x => x.EventId == eventId).Select(x => x.UserId).ToListAsync();
        }
    }
}
