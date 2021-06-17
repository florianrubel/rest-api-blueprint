namespace rest_api_blueprint.Models.Geo.Address
{
    public class ViewAddress : UuidViewModel
    {
        public string? Company { get; set; }

        public string CountryCode { get; set; }

        public char? Gender { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string Street { get; set; }

        public string Number { get; set; }

        public string PostalCode { get; set; }

        public string City { get; set; }

        public double? Lat { get; set; }

        public double? Lng { get; set; }

        public string? PlaceId { get; set; }

        public string UserId { get; set; }
    }
}
