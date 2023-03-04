namespace EventSharingApi.Repository
{
    public interface IEventAttendeeRepository
    {
        Task<bool> Add(int eventId, int userId, int quota);
        Task<int> GetEventNumberOfParticipants(int eventId);
        Task<List<int>> GetParticipantIds(int eventId);
    }
}
