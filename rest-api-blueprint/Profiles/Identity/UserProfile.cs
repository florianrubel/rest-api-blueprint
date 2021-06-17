using rest_api_blueprint.Entities.Identity;
using rest_api_blueprint.Models.Identity.User;
using rest_api_blueprint.Models.Social;
using AutoMapper;

namespace rest_api_blueprint.Profiles.Identity
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, ViewUser>();
            CreateMap<SignUpUser, User>().ForMember(d => d.UserName, opt => opt.MapFrom(s => s.Email));
            CreateMap<PatchUser, User>().ReverseMap();
            CreateMap<User, PublicUser>();
        }
    }
}
