namespace rest_api_blueprint.Models.Authentication.Google
{
    public class GoogleOAuthOptions
    {
        public string ClientId { get; set; }

        public string ClientKey { get; set; }

        public bool Debug { get; set; }

        public string RedirectUri { get; set; }
    }
}
