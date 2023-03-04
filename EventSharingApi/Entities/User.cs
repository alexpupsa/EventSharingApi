using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventSharingApi.Entities
{
    [Table("User")]
    public class User
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Column("Username")]
        public string? Username { get; set; }

        public ICollection<EventAttendee> AttendedEvents { get; set; }
    }
}
