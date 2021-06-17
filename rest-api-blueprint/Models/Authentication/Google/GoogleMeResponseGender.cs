namespace rest_api_blueprint.Models.Authentication.Google
{
    public class GoogleMeResponseGender
    {
        public GoogleMeResponseMetadata Metadata { get; set; }

        /// <summary>
        /// male, female or unspecified
        /// </summary>
        public string Value { get; set; }

        public string FormattedValue { get; set; }
    }
}
