using rest_api_blueprint.Constants.Geo;
using rest_api_blueprint.Models.Api;
using System;
using System.ComponentModel.DataAnnotations;

namespace rest_api_blueprint.Models.Social.Announcement
{
    public class AnnouncementSearchParameters : SearchParameters
    {
        public string? CreatorIds { get; set; }

        public double? Lat { get; set; }

        public double? Lng { get; set; }

        public bool? IsPublic { get; set; }

        /// <summary>
        /// In km (min 1km, max 20km)
        /// </summary>
        [Range(GeoConstants.RADIUS_MIN, GeoConstants.RADIUS_MAX)]
        public double? RadiusInKm { get; set; } = GeoConstants.RADIUS_MIN;

        public DateTimeOffset? CreatedAtFrom { get; set; }

        public DateTimeOffset? CreatedAtTo { get; set; }
    }
}
