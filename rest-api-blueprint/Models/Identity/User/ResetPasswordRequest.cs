using rest_api_blueprint.Constants;
using System.ComponentModel.DataAnnotations;

namespace rest_api_blueprint.Models.Identity.User
{
    public class ResetPasswordRequest
    {
        [Required]
        [MinLength(InputSizes.DEFAULT_TEXT_MIN_LENGTH)]
        [MaxLength(InputSizes.DEFAULT_TEXT_MAX_LENGTH)]
        public string UserName { get; set; }

        [Required]
        [MaxLength(InputSizes.TOKEN_MAX_LENGTH)]
        public string Token { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [MaxLength(InputSizes.DEFAULT_TEXT_MAX_LENGTH)]
        public string Password { get; set; }
    }
}
