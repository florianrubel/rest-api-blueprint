using rest_api_blueprint.Entities.Geo;
using rest_api_blueprint.Models.Geo.Address;
using AutoMapper;

namespace rest_api_blueprint.Profiles.Geo
{
    public class AddressProfile : Profile
    {
        public AddressProfile()
        {
            CreateMap<Address, ViewAddress>();
            CreateMap<CreateAddress, Address>();
            CreateMap<PatchAddress, Address>().ReverseMap();
        }
    }
}
