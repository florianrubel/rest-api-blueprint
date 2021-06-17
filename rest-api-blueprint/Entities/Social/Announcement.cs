using rest_api_blueprint.Constants;
using rest_api_blueprint.Entities.Identity;
using rest_api_blueprint.Entities.Geo;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace rest_api_blueprint.Entities.Social
{
    public class Announcement : UuidBaseEntity
    {
        [Required]
        [MinLength(InputSizes.MULTILINE_TEXT_MIN_LENGTH)]
        [MaxLength(InputSizes.MULTILINE_TEXT_MAX_LENGTH)]
        public string Bodytext { get; set; }

        [Required]
        [ForeignKey("CreatorId")]
        [MaxLength(InputSizes.DEFAULT_TEXT_MAX_LENGTH)]
        public string CreatorId { get; set; }
        [Required]
        public virtual User Creator { get; set; }

        public bool IsPublic { get; set; } = false;

        [Required]
        [ForeignKey("AddressId")]
        public Guid AddressId { get; set; }
        public virtual Address Address { get; set; }

        [Required]
        [MinLength(InputSizes.DEFAULT_TEXT_MIN_LENGTH)]
        [MaxLength(InputSizes.DEFAULT_TEXT_MAX_LENGTH)]
        public string Title { get; set; }
    }
}
