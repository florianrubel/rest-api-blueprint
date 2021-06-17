using System.Collections.Generic;

namespace rest_api_blueprint.Models.Geo.Google
{
    public class PlaceResult
    {
        public List<AddressComponent>? AddressComponents { get; set; }

        public string? AdrAddress { get; set; }

        public string? FormattedAddress { get; set; }

        public string? FormattedPhoneNumber { get; set; }

        public Geometry? Geometry { get; set; }

        public string? Icon { get; set; }

        public string? InternationalPhoneNumber { get; set; }

        public string? Name { get; set; }

        public string? PlaceId { get; set; }

        public double? Rating { get; set; }

        public string? Reference { get; set; }

        public List<Review>? Reviews { get; set; }

        public List<string>? Types { get; set; }

        public string? Url { get; set; }

        public int? UtcOffset { get; set; }

        public string? Vicinity { get; set; }

        public string? Website { get; set; }
    }
}
