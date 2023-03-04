using EventSharingApi.Models;

namespace EventSharingApi.Repository
{
    public interface IEventRepository
    {
        Task<bool> Add(EventModel eventModel);
        Task<EventModel> Get(int id);
        Task<List<EventModel>> GetAll();
        Task<List<EventModel>> GetAllEventsThatStartInMinutes(int minutes);
        Task<List<int>> GetAllEventIdsThatShouldBeMarkedAsFinished();
        Task MarkEventsAsFinished(List<int> eventIds);
    }
}
