using System.Collections.Generic;

namespace rest_api_blueprint.Models.Geo.Google
{
    public class Geometry
    {
        public Dictionary<string, LatLng>? Bounds { get; set; }

        public LatLng? Location { get; set; }

        public string? LocationType { get; set; }

        public Dictionary<string, LatLng> ViewPort { get; set; }
    }
}
