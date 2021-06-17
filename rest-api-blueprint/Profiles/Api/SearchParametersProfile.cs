using rest_api_blueprint.Models.Api;
using rest_api_blueprint.Models.Geo.Address;
using rest_api_blueprint.Models.Social.Announcement;
using AutoMapper;

namespace rest_api_blueprint.Profiles.Api
{
    public class SearchParametersProfile : Profile
    {
        public SearchParametersProfile()
        {
            CreateMap<SearchParameters, AddressSearchParameters>();
            CreateMap<SearchParameters, AnnouncementSearchParameters>();
        }
    }
}