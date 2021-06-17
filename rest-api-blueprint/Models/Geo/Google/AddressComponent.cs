using System.Collections.Generic;

namespace rest_api_blueprint.Models.Geo.Google
{
    public class AddressComponent
    {
        public string? LongName { get; set; }

        public string? ShortName { get; set; }

        public List<string>? Types { get; set; }
    }
}
