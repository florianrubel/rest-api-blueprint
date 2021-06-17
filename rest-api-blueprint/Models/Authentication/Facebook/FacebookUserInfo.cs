using System;

namespace rest_api_blueprint.Models.Authentication.Facebook
{
    public class FacebookUserInfo
    {
        public string Id { get; set; }


        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public DateTime? Birthday { get; set; }

        public string? Email { get; set; }

        /// <summary>
        /// male, female or something else
        /// </summary>
        public string? Gender { get; set; }

        public FacebookUserPicture? Picture { get; set; }
    }
}
