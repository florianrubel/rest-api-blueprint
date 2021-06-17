using System.Collections.Generic;

namespace rest_api_blueprint.Models.Geo.Google
{
    public class PlaceResponse
    {
        public List<string>? HtmlAttributions { get; set; }

        public PlaceResult? Result { get; set; }

        public string? Status { get; set; }
    }
}
