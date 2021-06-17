using rest_api_blueprint.Constants;
using System;
using System.ComponentModel.DataAnnotations;

namespace rest_api_blueprint.Models.Social.Announcement
{
    public class PatchAnnouncement
    {
        [MinLength(InputSizes.MULTILINE_TEXT_MIN_LENGTH)]
        [MaxLength(InputSizes.MULTILINE_TEXT_MAX_LENGTH)]
        public string Bodytext { get; set; }

        public bool IsPublic { get; set; } = false;

        public Guid AddressId { get; set; }

        [Required]
        [MinLength(InputSizes.DEFAULT_TEXT_MIN_LENGTH)]
        [MaxLength(InputSizes.DEFAULT_TEXT_MAX_LENGTH)]
        public string Title { get; set; }
    }
}
