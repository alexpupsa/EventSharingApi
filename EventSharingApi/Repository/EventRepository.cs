using AutoMapper;
using EventSharingApi.Context;
using EventSharingApi.Entities;
using EventSharingApi.Models;
using Microsoft.EntityFrameworkCore;

namespace EventSharingApi.Repository
{
    public class EventRepository : IEventRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;

        public EventRepository(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<bool> Add(EventModel eventModel)
        {
            var entity = _mapper.Map<Event>(eventModel);
            await _dbContext.Events.AddAsync(entity);
            var result = await _dbContext.SaveChangesAsync();
            return result > 0;
        }

        public async Task<EventModel> Get(int id)
        {
            var entity = await _dbContext.Events.FirstOrDefaultAsync(x => x.Id == id);
            return _mapper.Map<EventModel>(entity);
        }

        public async Task<List<EventModel>> GetAll()
        {
            var events = await _dbContext.Events.ToListAsync();
            return _mapper.Map<List<EventModel>>(events);
        }

        public async Task<List<int>> GetAllEventIdsThatShouldBeMarkedAsFinished()
        {
            return await _dbContext.Events.Where(x => x.DateTime.AddMinutes(x.DurationInMinutes) > DateTime.UtcNow).Select(x => x.Id).ToListAsync();
        }

        public async Task<List<EventModel>> GetAllEventsThatStartInMinutes(int minutes)
        {
            var now = DateTime.Now;
            var events = await _dbContext.Events.Where(x => (now - x.DateTime).TotalMinutes == minutes).ToListAsync();
            return _mapper.Map<List<EventModel>>(events);
        }

        public async Task MarkEventsAsFinished(List<int> eventIds)
        {
            var entities = _dbContext.Events.Where(x => eventIds.Contains(x.Id));
            foreach (var entity in entities)
            {
                entity.State = EventState.Finished;
            }
            await _dbContext.SaveChangesAsync();
        }
    }
}
