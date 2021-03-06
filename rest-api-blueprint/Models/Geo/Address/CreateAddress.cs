using rest_api_blueprint.Constants;
using System.ComponentModel.DataAnnotations;

namespace rest_api_blueprint.Models.Geo.Address
{
    public class CreateAddress
    {
        [MinLength(InputSizes.DEFAULT_TEXT_MIN_LENGTH)]
        [MaxLength(InputSizes.DEFAULT_TEXT_MAX_LENGTH)]
        public string? Company { get; set; }

        [Required]
        [MinLength(InputSizes.COUNTRY_CODE_LENGTH)]
        [MaxLength(InputSizes.COUNTRY_CODE_LENGTH)]
        public string CountryCode { get; set; }

        /// <summary>
        /// m = male
        /// f = female
        /// d = diverse
        /// </summary>
        public char? Gender { get; set; }

        [MinLength(InputSizes.DEFAULT_TEXT_MIN_LENGTH)]
        [MaxLength(InputSizes.DEFAULT_TEXT_MAX_LENGTH)]
        public string? FirstName { get; set; }

        [MinLength(InputSizes.DEFAULT_TEXT_MIN_LENGTH)]
        [MaxLength(InputSizes.DEFAULT_TEXT_MAX_LENGTH)]
        public string? LastName { get; set; }

        [Required]
        [MinLength(InputSizes.DEFAULT_TEXT_MIN_LENGTH)]
        [MaxLength(InputSizes.DEFAULT_TEXT_MAX_LENGTH)]
        public string Street { get; set; }

        [Required]
        [MinLength(InputSizes.DEFAULT_TEXT_MIN_LENGTH)]
        [MaxLength(InputSizes.DEFAULT_TEXT_MAX_LENGTH)]
        public string PostalCode { get; set; }

        [Required]
        [MinLength(InputSizes.DEFAULT_TEXT_MIN_LENGTH)]
        [MaxLength(InputSizes.DEFAULT_TEXT_MAX_LENGTH)]
        public string City { get; set; }

        public string? PlaceId { get; set; }
    }
}
