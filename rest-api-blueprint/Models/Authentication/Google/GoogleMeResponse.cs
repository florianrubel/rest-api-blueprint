using System.Collections.Generic;

namespace rest_api_blueprint.Models.Authentication.Google
{
    public class GoogleMeResponse
    {
        public string ResourceName { get; set; }

        public string Etag { get; set; }

        public List<GoogleMeResponseGender>? Genders { get; set; }

        public List<GoogleMeResponseBirthday>? Birthdays { get; set; }
    }
}
