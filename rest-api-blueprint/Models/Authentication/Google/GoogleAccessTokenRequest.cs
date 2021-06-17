namespace rest_api_blueprint.Models.Authentication.Google
{
    public class GoogleAccessTokenRequest
    {
        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string Code { get; set; }

        public string GrantType { get; set; }

        public string RedirectUri { get; set; }
    }
}
