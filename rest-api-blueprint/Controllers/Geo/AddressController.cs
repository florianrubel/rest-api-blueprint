using rest_api_blueprint.Constants.Identity;
using rest_api_blueprint.Entities.Geo;
using rest_api_blueprint.Helpers;
using rest_api_blueprint.Models.Api;
using rest_api_blueprint.Models.Geo.Address;
using rest_api_blueprint.Repositories.Geo;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rest_api_blueprint.Controllers.Geo
{
    [Route("geo/[controller]")]
    public class AddressController : DefaultControllerTemplate
    {
        private readonly IAddressRepository<Address> _addressRepository;
        private readonly IMapper _mapper;


        public AddressController(
            IAddressRepository<Address> addressRepository,
            IMapper mapper
        )
        {
            _addressRepository = addressRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Get a paginated list with filters.
        /// </summary>
        [HttpGet]
        [Route("")]
        public async Task<ActionResult<PagedList<ViewAddress>>> GetMultiple([FromQuery] AddressSearchParameters parameters)
        {
            PagedList<Address> addresses = await _addressRepository.GetMultiple(parameters);
            SetPaginationHeaders(addresses);

            return Ok(_mapper.Map<IEnumerable<ViewAddress>>(addresses).ShapeData(parameters.Fields));
        }

        /// <summary>
        /// Get a list with multiple ids.
        /// </summary>
        [HttpGet]
        [Route("({ids})")]
        public async Task<ActionResult<IEnumerable<ViewAddress>>> GetMultiple(
            [FromRoute][ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<Guid> ids,
            [FromQuery] ShapingParameters parameters
        )
        {
            if (ids == null)
                return BadRequest();

            IEnumerable<Address> addresses = await _addressRepository.GetMultiple(ids, parameters);

            return Ok(_mapper.Map<IEnumerable<ViewAddress>>(addresses).ShapeData(parameters.Fields));
        }

        /// <summary>
        /// Get by id.
        /// </summary>
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<ViewAddress>> GetOne(Guid id, [FromQuery] string? fields)
        {
            Address? entity = await _addressRepository.GetOneOrDefault(id);
            if (entity == null)
                return NotFound();

            return Ok(_mapper.Map<ViewAddress>(entity).ShapeData(fields));
        }

        /// <summary>
        /// Geocode all.
        /// </summary>
        [HttpPatch]
        [Route("geocode-all")]
        [Authorize(Roles = RoleConstants.ADMINISTRATOR)]
        public async Task<ActionResult<ViewAddress>> GeocodeAll()
        {
            PagedList<Address> entities = await _addressRepository.GetMultiple(new AddressSearchParameters { PageSize = -1 });
            await _addressRepository.UpdateRange(entities);
            return Ok();
        }

        /// <summary>
        /// Create a new one.
        /// </summary>
        [HttpPost]
        [Route("")]
        [Authorize(Roles = RoleConstants.ADMIN_OR_MODERATOR)]
        public async Task<ActionResult<ViewAddress>> Create([FromBody] CreateAddress createObj)
        {
            string? currentUserId = GetCurrentUserId();

            if (currentUserId == null)
                return Unauthorized();

            Address newEntity = _mapper.Map<Address>(createObj);
            newEntity.UserId = currentUserId;
            newEntity = await _addressRepository.Create(newEntity);

            return Ok(_mapper.Map<ViewAddress>(newEntity));
        }

        /// <summary>
        /// Patch an existing one.
        /// </summary>
        [HttpPatch]
        [Route("{id}")]
        [Authorize(Roles = RoleConstants.ADMIN_OR_MODERATOR)]
        public async Task<ActionResult<ViewAddress>> Patch(Guid id, [FromBody] JsonPatchDocument<PatchAddress> patchDocument)
        {
            Address? entity = await _addressRepository.GetOneOrDefault(id);

            if (entity == null)
                return NotFound();

            if (!CurrentUserHasRole(RoleConstants.ADMINISTRATOR))
            {
                if (entity.UserId != GetCurrentUserId())
                {
                    return Forbid();
                }
            }

            PatchAddress patchObj = _mapper.Map<PatchAddress>(entity);
            patchDocument.ApplyTo(patchObj, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _mapper.Map(patchObj, entity);
            await _addressRepository.Update(entity);

            return Ok(_mapper.Map<ViewAddress>(entity));
        }

        /// <summary>
        /// Delete an existing one.
        /// </summary>
        [HttpDelete]
        [Route("{id}")]
        [Authorize(Roles = RoleConstants.ADMINISTRATOR)]
        public async Task<ActionResult> Delete(Guid id)
        {
            Address? entity = await _addressRepository.GetOneOrDefault(id);

            if (entity == null)
                return NotFound();

            await _addressRepository.Delete(entity);

            return Ok();
        }
    }
}
