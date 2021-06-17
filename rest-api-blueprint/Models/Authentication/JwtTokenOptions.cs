namespace rest_api_blueprint.Models.Authentication
{
    public class JwtTokenOptions
    {
        public string Issuer { get; set; }

        public string Audience { get; set; }

        public string Key { get; set; }

        public int BearerTTL { get; set; }

        public int RefreshTTL { get; set; }
    }
}
