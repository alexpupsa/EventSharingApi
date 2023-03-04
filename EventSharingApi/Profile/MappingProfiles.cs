using EventSharingApi.Entities;
using EventSharingApi.Models;

namespace EventSharingApi.Profile
{
    public class MappingProfiles : AutoMapper.Profile
    {
        public MappingProfiles()
        {
            CreateMap<Event, EventModel>();
            CreateMap<EventModel, Event>();
        }
    }
}
