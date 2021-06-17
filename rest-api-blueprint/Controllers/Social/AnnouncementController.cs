using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using rest_api_blueprint.Constants.Identity;
using rest_api_blueprint.Entities.Geo;
using rest_api_blueprint.Entities.Social;
using rest_api_blueprint.Helpers;
using rest_api_blueprint.Models.Api;
using rest_api_blueprint.Models.Social.Announcement;
using rest_api_blueprint.Repositories.Geo;
using rest_api_blueprint.Repositories.Social;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rest_api_blueprint.Controllers.Social
{
    [Route("social/[controller]")]
    public class AnnouncementController : DefaultControllerTemplate
    {
        private readonly IAnnouncementRepository<Announcement> _announcementRepository;
        private readonly IAddressRepository<Address> _addressRepository;
        private readonly IMapper _mapper;

        public AnnouncementController(
            IAnnouncementRepository<Announcement> announcementRepository,
            IAddressRepository<Address> addressRepository,
            IMapper mapper
        )
        {
            _announcementRepository = announcementRepository;
            _addressRepository = addressRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Get a paginated list with filters.
        /// </summary>
        [HttpGet]
        [Route("")]
        public async Task<ActionResult<PagedList<ViewAnnouncement>>> GetMultiple([FromQuery] AnnouncementSearchParameters parameters)
        {
            string? currentUserId = GetCurrentUserId();

            if (currentUserId == null)
                return Unauthorized();

            if (!CurrentUserHasRole(RoleConstants.ADMINISTRATOR))
            {
                if (parameters.CreatorIds != null)
                {
                    List<string> userIds = parameters.CreatorIds.Split(',').ToList();
                    if (userIds.Count() > 1)
                    {
                        parameters.IsPublic = true;
                    } else
                    {
                        if (userIds.FirstOrDefault() != currentUserId)
                        {
                            parameters.IsPublic = true;
                        }
                    }
                } else
                {
                    parameters.IsPublic = true;
                }
            }

            PagedList<Announcement> entities = await _announcementRepository.GetMultiple(parameters);
            SetPaginationHeaders(entities);

            return Ok(_mapper.Map<IEnumerable<ViewAnnouncement>>(entities).ShapeData(parameters.Fields));
        }

        /// <summary>
        /// Get a list with multiple ids.
        /// </summary>
        [HttpGet]
        [Route("({ids})")]
        public async Task<ActionResult<IEnumerable<ViewAnnouncement>>> GetMultiple(
            [FromRoute][ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<Guid> ids,
            [FromQuery] ShapingParameters parameters
        )
        {
            if (ids == null)
                return BadRequest();

            string? currentUserId = GetCurrentUserId();

            if (currentUserId == null)
                return Unauthorized();

            IEnumerable<Announcement> entities = await _announcementRepository.GetMultiple(ids, parameters);

            if (!CurrentUserHasRole(RoleConstants.ADMINISTRATOR)) {
                entities = entities.Where(e => e.CreatorId == currentUserId || e.IsPublic);
            }

            return Ok(_mapper.Map<IEnumerable<Announcement>>(entities).ShapeData(parameters.Fields));
        }

        /// <summary>
        /// Get by id.
        /// </summary>
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<ViewAnnouncement>> GetOne(Guid id, [FromQuery] string? fields)
        {
            Announcement? entity = await _announcementRepository.GetOneOrDefault(id);
            if (entity == null)
                return NotFound();

            string? currentUserId = GetCurrentUserId();

            if (currentUserId == null)
                return Unauthorized();

            if (!CurrentUserHasRole(RoleConstants.ADMINISTRATOR))
            {
                if (entity.CreatorId != currentUserId && !entity.IsPublic)
                    return Forbid();
            }

            return Ok(_mapper.Map<ViewAnnouncement>(entity).ShapeData(fields));
        }

        /// <summary>
        /// Create a new one.
        /// </summary>
        [HttpPost]
        [Route("")]
        [Authorize(Roles = RoleConstants.ADMIN_OR_MODERATOR)]
        public async Task<ActionResult<ViewAnnouncement>> Create([FromBody] CreateAnnouncement createObj)
        {
            string? currentUserId = GetCurrentUserId();

            if (currentUserId == null)
                return Unauthorized();

            Address? address = await _addressRepository.GetOneOrDefault((Guid)createObj.AddressId);

            if (address == null)
            {
                return BadRequest(new ErrorResponse(new List<string> { ErrorCodes.ADDRESS_NOT_FOUND }));
            }
            else if(!CurrentUserHasRole(RoleConstants.ADMINISTRATOR) && address.UserId != currentUserId)
            {
                return BadRequest(new ErrorResponse(new List<string> { ErrorCodes.ADDRESS_BELONGS_TO_ANOTHER_USER }));
            }

            Announcement newEntity = _mapper.Map<Announcement>(createObj);
            newEntity.CreatorId = GetCurrentUserId();
            newEntity = await _announcementRepository.Create(newEntity);

            return Ok(_mapper.Map<ViewAnnouncement>(newEntity));
        }

        /// <summary>
        /// Patch an existing one.
        /// </summary>
        [HttpPatch]
        [Route("{id}")]
        [Authorize(Roles = RoleConstants.ADMIN_OR_MODERATOR)]
        public async Task<ActionResult<ViewAnnouncement>> Patch(Guid id, [FromBody] JsonPatchDocument<PatchAnnouncement> patchDocument)
        {
            string? currentUserId = GetCurrentUserId();

            if (currentUserId == null)
                return Unauthorized();

            Announcement? entity = await _announcementRepository.GetOneOrDefault(id);

            if (entity == null)
                return NotFound();

            if (!CurrentUserHasRole(RoleConstants.ADMINISTRATOR))
            {
                if (entity.CreatorId != GetCurrentUserId())
                {
                    return Forbid();
                }
            }

            PatchAnnouncement patchObj = _mapper.Map<PatchAnnouncement>(entity);

            patchDocument.ApplyTo(patchObj, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Address? address = await _addressRepository.GetOneOrDefault((Guid)patchObj.AddressId);

            if (address == null)
            {
                return BadRequest(new ErrorResponse(new List<string> { ErrorCodes.ADDRESS_NOT_FOUND }));
            }
            else if (!CurrentUserHasRole(RoleConstants.ADMINISTRATOR) && address.UserId != currentUserId)
            {
                return BadRequest(new ErrorResponse(new List<string> { ErrorCodes.ADDRESS_BELONGS_TO_ANOTHER_USER }));
            }

            _mapper.Map(patchObj, entity);
            await _announcementRepository.Update(entity);

            return Ok(_mapper.Map<ViewAnnouncement>(entity));
        }

        /// <summary>
        /// Delete an existing one.
        /// </summary>
        [HttpDelete]
        [Route("{id}")]
        [Authorize(Roles = RoleConstants.ADMIN_OR_MODERATOR)]
        public async Task<ActionResult> Delete(Guid id)
        {
            string? currentUserId = GetCurrentUserId();

            if (currentUserId == null)
                return Unauthorized();

            Announcement? entity = await _announcementRepository.GetOneOrDefault(id);

            if (entity == null)
                return NotFound();

            if (!CurrentUserHasRole(RoleConstants.ADMINISTRATOR))
            {
                if (entity.CreatorId != currentUserId)
                {
                    return Forbid();
                }
            }

            await _announcementRepository.Delete(entity);

            return Ok();
        }
    }
}