using AutoMapper;
using rest_api_blueprint.Entities.Social;
using rest_api_blueprint.Models.Social.Announcement;

namespace rest_api_blueprint.Profiles.Social
{
    public class AnnouncementProfile : Profile
    {
        public AnnouncementProfile()
        {
            CreateMap<Announcement, ViewAnnouncement>();
            CreateMap<CreateAnnouncement, Announcement>();
            CreateMap<PatchAnnouncement, Announcement>().ReverseMap();
        }
    }
}
