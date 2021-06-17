using rest_api_blueprint.Constants;
using System;
using System.ComponentModel.DataAnnotations;

namespace rest_api_blueprint.Models.Identity.User
{
    public class SignUpUser
    {
        [Required]
        [MinLength(InputSizes.DEFAULT_TEXT_MIN_LENGTH)]
        [MaxLength(InputSizes.DEFAULT_TEXT_MAX_LENGTH)]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [MaxLength(InputSizes.DEFAULT_TEXT_MAX_LENGTH)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public DateTime? Birthday { get; set; }

        [Required]
        [MinLength(InputSizes.DEFAULT_TEXT_MIN_LENGTH)]
        [MaxLength(InputSizes.DEFAULT_TEXT_MAX_LENGTH)]
        public string FirstName { get; set; }

        public char? Gender { get; set; }

        [MinLength(InputSizes.DEFAULT_TEXT_MIN_LENGTH)]
        [MaxLength(InputSizes.DEFAULT_TEXT_MAX_LENGTH)]
        public string? LastName { get; set; }
    }
}
