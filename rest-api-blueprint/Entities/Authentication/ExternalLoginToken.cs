using rest_api_blueprint.Constants;
using rest_api_blueprint.Entities.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace rest_api_blueprint.Entities.Authentication
{
    public class ExternalLoginToken : UuidBaseEntity
    {
        [MaxLength(InputSizes.DEFAULT_TEXT_MAX_LENGTH)]
        [ForeignKey("UserId")]
        public string? UserId { get; set; }
        public virtual User? User { get; set; }

        [Required]
        [MaxLength(InputSizes.DEFAULT_TEXT_MAX_LENGTH)]
        public string Token { get; set; }

        public DateTimeOffset ExpiresAt { get; set; }

        [MaxLength(InputSizes.DEFAULT_TEXT_MAX_LENGTH)]
        public string LoginProvider { get; set; }

        [MaxLength(InputSizes.DEFAULT_TEXT_MAX_LENGTH)]
        public string? IpAddress { get; set; }
    }
}
