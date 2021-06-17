namespace rest_api_blueprint.Models.Authentication.Facebook
{
    public class FacebookAccessTokenResponse
    {
        public string AccessToken { get; set; }

        public string TokenType { get; set; }

        public int ExpiresIn { get; set; }
    }
}
