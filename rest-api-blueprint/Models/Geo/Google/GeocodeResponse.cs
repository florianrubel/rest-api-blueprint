using System.Collections.Generic;

namespace rest_api_blueprint.Models.Geo.Google
{
    public class GeocodeResponse
    {
        public List<GeocodeResult>? Results { get; set; }

        public string? Status { get; set; }
    }
}
