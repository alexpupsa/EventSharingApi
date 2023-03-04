using EventSharingApi.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventSharingApi.Entities
{
    [Table("Event")]
    public class Event
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Column("Title")]
        public string? Title { get; set; }

        [Column("Description")]
        public string? Description { get; set; }

        [Column("State")]
        public EventState State { get; set; }

        [Column("DateTime")]
        public DateTimeOffset DateTime { get; set; }

        [Column("DurationInMinutes")]
        public int DurationInMinutes { get; set; }

        [Column("Quota")]
        public int Quota { get; set; }

        public ICollection<EventAttendee> Attendees { get; set; }
    }
}
