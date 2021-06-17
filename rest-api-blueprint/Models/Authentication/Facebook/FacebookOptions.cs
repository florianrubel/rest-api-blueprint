namespace rest_api_blueprint.Models.Authentication.Facebook
{
    public class FacebookOptions
    {
        public string AppId { get; set; }

        public string AppSecret { get; set; }

        public bool Debug { get; set; }

        public string RedirectUri { get; set; }
    }
}
