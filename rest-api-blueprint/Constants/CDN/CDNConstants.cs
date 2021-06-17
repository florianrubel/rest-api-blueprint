using System.Collections.Generic;

namespace rest_api_blueprint.Constants.CDN
{
    public class CDNConstants
    {
        public const string DIRECTORY_PROFILE_PICTURES = "profile-pictures";

        public static readonly List<string> TYPES_IMAGES = new List<string>
        {
            "image/jpeg",
            "image/jpg",
            "image/png",
        };
    }
}
