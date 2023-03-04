using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventSharingApi.Entities
{
    public class EventAttendee
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [ForeignKey("Event")]
        public int EventId { get; set; }
        public Event Event { get; set; }

        [ForeignKey("User")]

        public int UserId { get; set; }
        public User User { get; set; }
    }
}
