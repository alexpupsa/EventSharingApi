namespace EventSharingApi.Models
{
    public class EventModel
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public EventState State { get; set; }
        public DateTimeOffset DateTime { get; set; }
        public int DurationInMinutes { get; set; }
        public int Quota { get; set; }
    }
}
