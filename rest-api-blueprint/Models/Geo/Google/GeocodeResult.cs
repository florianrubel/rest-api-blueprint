using System.Collections.Generic;

namespace rest_api_blueprint.Models.Geo.Google
{
    public class GeocodeResult
    {
        public List<AddressComponent>? AddressComponents { get; set; }

        public string? FormattedAddress { get; set; }

        public Geometry? Geometry { get; set; }

        public string? PlaceId { get; set; }

        public List<string> Types { get; set; }

        public PlaceResult? PlaceResult { get; set; }
    }
}
