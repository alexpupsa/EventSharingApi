using EventSharingApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventSharingApi.Context
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public DbSet<Event> Events { get; set; }

        public DbSet<EventAttendee> EventAttendees { get; set; }

        public AppDbContext() : base()
        {

        }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
    }
}
