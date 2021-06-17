using rest_api_blueprint.Entities.Identity;
using rest_api_blueprint.Models.Identity.Role;
using AutoMapper;

namespace rest_api_blueprint.Profiles.Identity
{
    public class RoleProfile : Profile
    {
        public RoleProfile()
        {
            CreateMap<Role, ViewRole>();
            CreateMap<CreateRole, Role>();
            CreateMap<PatchRole, Role>().ReverseMap();
        }
    }
}
