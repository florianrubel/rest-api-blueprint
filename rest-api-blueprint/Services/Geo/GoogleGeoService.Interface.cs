using rest_api_blueprint.Entities.Geo;
using rest_api_blueprint.Models.Geo.Google;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace rest_api_blueprint.Services.Geo
{
    public interface IGoogleGeoService
    {
        Task<List<GeocodeResult>> GetGeoCodeResults(string searchString, string language);
        Task GeocodeAddress(Address address);
    }
}