using rest_api_blueprint.Constants;
using System.ComponentModel.DataAnnotations;

namespace rest_api_blueprint.Models.Identity.Role
{
    public class CreateRole
    {
        [Required]
        [MinLength(InputSizes.DEFAULT_TEXT_MIN_LENGTH)]
        [MaxLength(InputSizes.DEFAULT_TEXT_MAX_LENGTH)]
        public string Name { get; set; }
    }
}
