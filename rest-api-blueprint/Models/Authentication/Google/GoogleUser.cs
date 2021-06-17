namespace rest_api_blueprint.Models.Authentication.Google
{
    public class GoogleUser
    {
        public GoogleUserInfo PublicProfile { get; set; }

        public GoogleMeResponse AdditionalRessources { get; set; }
    }
}
