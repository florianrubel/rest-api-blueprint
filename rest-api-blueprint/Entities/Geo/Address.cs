using rest_api_blueprint.Constants;
using rest_api_blueprint.Entities.Identity;
using NetTopologySuite.Geometries;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace rest_api_blueprint.Entities.Geo
{
    public class Address : UuidBaseEntity
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

        public double? Lat { get; set; }

        public double? Lng { get; set; }

        public virtual Point? Location { get; set; }

        [MaxLength(InputSizes.DEFAULT_TEXT_MAX_LENGTH)]
        public string? PlaceId { get; set; }

        [Required]
        [ForeignKey("UserId")]
        [MaxLength(InputSizes.DEFAULT_TEXT_MAX_LENGTH)]
        public string UserId { get; set; }
        [Required]
        public virtual User User { get; set; }
    }
}
