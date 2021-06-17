using rest_api_blueprint.Constants;
using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;

namespace rest_api_blueprint.Entities.Identity
{
    public class User : IdentityUser
    {
        [MaxLength(InputSizes.MULTILINE_TEXT_MAX_LENGTH)]
        public string? AboutMe { get; set; }

        [Url]
        [MaxLength(InputSizes.DEFAULT_TEXT_MAX_LENGTH)]
        public string? AvatarUri { get; set; }

        public DateTime? Birthday { get; set; }

        [MaxLength(InputSizes.DEFAULT_TEXT_MAX_LENGTH)]
        public string? FacebookUserId { get; set; }

        [MaxLength(InputSizes.DEFAULT_TEXT_MAX_LENGTH)]
        public string? FanOf { get; set; }

        [Required]
        [MaxLength(InputSizes.DEFAULT_TEXT_MAX_LENGTH)]
        public string FirstName { get; set; }

        /// <summary>
        /// m = male
        /// f = female
        /// d = diverse
        /// </summary>
        [MaxLength(1)]
        public char? Gender { get; set; }

        [MaxLength(InputSizes.DEFAULT_TEXT_MAX_LENGTH)]
        public string? GoogleUserId { get; set; }

        public DateTimeOffset? LastLogin { get; set; }

        [MaxLength(InputSizes.DEFAULT_TEXT_MAX_LENGTH)]
        public string? LastName { get; set; }

        public uint? OldId { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
