using System.Collections.Generic;

namespace rest_api_blueprint.Models.Authentication.Facebook
{
    public class FacebookAccessTokenContentData
    {
        public long? AppId { get; set; }

        public string Type { get; set; }

        public string Application { get; set; }

        public int ExpiresAt { get; set; }

        public bool IsValid { get; set; }

        public int IssuedAt { get; set; }

        public Dictionary<string, string> Metadata { get; set; }

        public List<string> Scopes { get; set; }

        public string UserId { get; set; }
    }
}
