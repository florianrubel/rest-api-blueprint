using rest_api_blueprint.Models.Api;

namespace rest_api_blueprint.Models.Geo.Address
{
    public class AddressSearchParameters : SearchParameters
    {
        public string? UserIds { get; set; }
    }
}
