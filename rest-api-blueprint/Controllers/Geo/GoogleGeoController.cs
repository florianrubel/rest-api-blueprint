using rest_api_blueprint.Models.Geo.Google;
using rest_api_blueprint.Services.Geo;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace rest_api_blueprint.Controllers.Geo
{
    [Route("geo/[controller]")]
    public class GoogleGeoController : DefaultControllerTemplate
    {
        private readonly IGoogleGeoService _googleGeoService;
        private readonly IMapper _mapper;


        public GoogleGeoController (
            IGoogleGeoService googleGeoService,
            IMapper mapper
        )
        {
            _googleGeoService = googleGeoService;
            _mapper = mapper;
        }

        /// <summary>
        /// Try to geocode address.
        /// </summary>
        [HttpGet]
        [Route("")]
        [AllowAnonymous]
        public async Task<ActionResult<List<GeocodeResult>>> GetGeoCodeResults([FromQuery] string searchString, [FromQuery] string language = "en")
        {
            return Ok(await _googleGeoService.GetGeoCodeResults(searchString, language));
        }
    }
}
