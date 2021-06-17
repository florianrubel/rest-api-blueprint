using rest_api_blueprint.Constants.CDN;
using rest_api_blueprint.Constants.Identity;
using rest_api_blueprint.Entities.Identity;
using rest_api_blueprint.Helpers;
using rest_api_blueprint.Models.Api;
using rest_api_blueprint.Models.Identity.User;
using rest_api_blueprint.Repositories.Identity;
using rest_api_blueprint.Services.CDN;
using rest_api_blueprint.Services.Email;
using rest_api_blueprint.StaticServices;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace rest_api_blueprint.Controllers.Identity
{
    [Route("identity/[controller]")]
    public class UserController : DefaultControllerTemplate
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IOptions<AdminUserInitializerOptions> _adminUserInitializerOptions;

        public UserController(
            IMapper mapper,
            IEmailService emailService,
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            ICloudinaryService cloudinaryService,
            IOptions<AdminUserInitializerOptions> adminUserInitializerOptions
        )
        {
            _mapper = mapper;
            _emailService = emailService;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _cloudinaryService = cloudinaryService;
            this._adminUserInitializerOptions = adminUserInitializerOptions;
        }


        /// <summary>
        /// Get a paginated list with filters.
        /// </summary>
        [HttpGet]
        [Route("")]
        [Authorize(Roles = RoleConstants.ADMINISTRATOR)]
        public async Task<ActionResult<PagedList<ViewUser>>> GetMultiple([FromQuery] SearchParameters parameters)
        {
            PagedList<User> entities = await _userRepository.GetMultiple(parameters);
            SetPaginationHeaders(entities);

            var viewUsers = _mapper.Map<IEnumerable<ViewUser>>(entities);

            foreach (ViewUser viewUser in viewUsers)
            {
                User? user = (from entity in entities where entity.Id == viewUser.Id select entity).FirstOrDefault();
                if (user != null) {
                    viewUser.Roles = (List<string>)(await _userRepository.GetUserRoles(user));
                }
            }

            return Ok(viewUsers.ShapeData(parameters.Fields));
        }

        /// <summary>
        /// Get a list with multiple ids.
        /// </summary>
        [HttpGet]
        [Route("({ids})")]
        [Authorize(Roles = RoleConstants.ADMINISTRATOR)]
        public async Task<ActionResult<IEnumerable<ViewUser>>> GetMultiple(
            [FromRoute][ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<string> ids,
            [FromQuery] ShapingParameters parameters
        )
        {
            if (ids == null)
                return BadRequest();

            IEnumerable<User> entities = await _userRepository.GetMultiple(ids, parameters);

            var viewUsers = _mapper.Map<IEnumerable<ViewUser>>(entities);

            foreach (ViewUser viewUser in viewUsers)
            {
                User? user = (from entity in entities where entity.Id == viewUser.Id select entity).FirstOrDefault();
                if (user != null)
                {
                    viewUser.Roles = (List<string>)(await _userRepository.GetUserRoles(user));
                }
            }

            return Ok(viewUsers.ShapeData(parameters.Fields));
        }

        /// <summary>
        /// Get one by id.
        /// </summary>
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<ViewUser>> GetOne(Guid id)
        {
            User? entity = await _userRepository.GetOneOrDefault(id.ToString());

            if (entity == null)
                return NotFound();

            if (!CurrentUserHasRole(RoleConstants.ADMINISTRATOR))
            {
                if (GetCurrentUserId() != entity.Id)
                {
                    return Forbid();
                }
            }

            ViewUser viewUser = _mapper.Map<ViewUser>(entity);
            viewUser.Roles = (List<string>)(await _userRepository.GetUserRoles(entity));
            return Ok(viewUser);
        }

        /// <summary>
        /// Patch an existing one.
        /// </summary>
        [HttpPatch]
        [Route("{id}")]
        [Authorize(Roles = RoleConstants.ADMINISTRATOR)]
        public async Task<ActionResult<ViewUser>> Patch(Guid id, [FromBody] JsonPatchDocument<PatchUser> patchDocument)
        {
            User? entity = await _userRepository.GetOneOrDefault(id.ToString());

            if (entity == null)
                return NotFound();

            if (!CurrentUserHasRole(RoleConstants.ADMINISTRATOR))
            {
                if (GetCurrentUserId() != entity.Id)
                {
                    return Forbid();
                }
            }

            PatchUser patchObj = _mapper.Map<PatchUser>(entity);
            patchDocument.ApplyTo(patchObj, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            };

            _mapper.Map(patchObj, entity);

            if (entity.AvatarUri != null && entity.AvatarUri.Trim() == "")
            {
                entity.AvatarUri = null;
            }

            if (entity.Email != entity.UserName)
            {
                entity.UserName = entity.Email;
            }

            await _userRepository.Update(entity);

            return Ok(_mapper.Map<ViewUser>(entity));
        }

        /// <summary>
        /// Resend the order confirmation email.
        /// </summary>
        [HttpPost]
        [Route("{id}/resend-confirmation-email")]
        [Authorize(Roles = RoleConstants.ADMINISTRATOR)]
        public async Task<ActionResult> ResendConfirmationEmail(Guid id)
        {
            User? user = await _userRepository.GetOneOrDefault(id.ToString());

            if (user == null)
                return NotFound();

            string token = await _userRepository.GetEmailConfirmationToken(user);
            string base64Token = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(token));
            await _emailService.SendEmailConfirmationMail(user.Id, user.UserName, user.Email, base64Token);

            return Ok();
        }

        /// <summary>
        /// Lock or unlock user.
        /// </summary>
        [HttpPatch]
        [Route("{id}/lock-unlock")]
        [Authorize(Roles = RoleConstants.ADMINISTRATOR)]
        public async Task<ActionResult> LockOrUnlockUser(Guid id, [FromBody] LockUnlockRequest lockUnlockRequest)
        {
            User? user = await _userRepository.GetOneOrDefault(id.ToString());

            if (user != null)
            {
                IdentityResult result = await _userRepository.SetUserLockout(user, lockUnlockRequest.LockUser);
                if (result.Succeeded)
                {
                    if (lockUnlockRequest.LockUser)
                        await _emailService.SendAccountDeactivatedMail(user.UserName, user.Email);
                    else
                        await _emailService.SendAccountActivatedMail(user.UserName, user.Email);

                    return Ok();
                }

                return BadRequest(result.Errors);
            }

            return NotFound();
        }

        /// <summary>
        /// Assign user to role.
        /// </summary>
        [HttpPatch]
        [Route("{id}/assign-user-to-role")]
        [Authorize(Roles = RoleConstants.ADMINISTRATOR)]
        public async Task<ActionResult> AssignUserToRole(Guid id, [FromBody] UserRoleChangeRequest userRoleChangeRequest)
        {
            User? user = await _userRepository.GetOneOrDefault(id.ToString());
            Role? role = await _roleRepository.GetOneOrDefault(userRoleChangeRequest.RoleId.ToString());


            if (user == null || role == null)
                return NotFound();

            IdentityResult result = await _userRepository.AssignUserToRole(user, role);
            if (result.Succeeded)
            {
                return Ok();
            }
            return BadRequest(result.Errors);
        }

        /// <summary>
        /// Remove user from role.
        /// </summary>
        [HttpPatch]
        [Route("{userId}/remove-user-from-role")]
        [Authorize(Roles = RoleConstants.ADMINISTRATOR)]
        public async Task<ActionResult> RemoveUserFromRole(Guid userId, [FromBody] UserRoleChangeRequest userRoleChangeRequest)
        {
            User? user = await _userRepository.GetOneOrDefault(userId.ToString());
            Role? role = await _roleRepository.GetOneOrDefault(userRoleChangeRequest.RoleId.ToString());

            if (user == null || role == null)
                return NotFound();

            IdentityResult result = await _userRepository.RemoveUserFromRole(user, role);
            if (result.Succeeded)
            {
                return Ok();
            }
            return BadRequest(result.Errors);
        }

        /// <summary>
        /// Create admin user an role
        /// </summary>
        [HttpPatch]
        [Route("initialize")]
        [AllowAnonymous]
        public async Task<ActionResult> InitializeAdminUserAndRoles()
        {
            foreach (string roleName in RoleConstants.ROLES)
            {
                Role? role = await _roleRepository.GetOneOrDefaultByMame(roleName);
                if (role == null)
                {
                    role = new Role { Name = RoleConstants.ADMINISTRATOR };
                    await _roleRepository.Create(role);
                }
            }

            User? adminUser = await _userRepository.GetOneOrDefaultByName(_adminUserInitializerOptions.Value.Email);
            if (adminUser == null)
            {
                adminUser = new User
                {
                    UserName = _adminUserInitializerOptions.Value.Email,
                    Email = _adminUserInitializerOptions.Value.Email,
                    Gender = _adminUserInitializerOptions.Value.Gender,
                    FirstName = _adminUserInitializerOptions.Value.FirstName,
                    LastName = _adminUserInitializerOptions.Value.LastName,
                    Birthday = DateTime.Parse(_adminUserInitializerOptions.Value.BirthdayFromIso)
                };
                IdentityResult result = await _userRepository.Create(adminUser, TextService.GeneratePassword());
                if (result.Succeeded)
                {
                    string token = await _userRepository.GetEmailConfirmationToken(adminUser);
                    string base64Token = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(token));
                    await _emailService.SendEmailConfirmationMail(adminUser.Id, adminUser.UserName, adminUser.Email, base64Token);

                    adminUser.EmailConfirmed = true;
                    adminUser.LockoutEnabled = false;
                    await _userRepository.Update(adminUser);
                    return Ok();
                }
                else
                {
                    return BadRequest(result.Errors);
                }
            }

            Role? adminRole = await _roleRepository.GetOneOrDefaultByMame(RoleConstants.ADMINISTRATOR);

            IdentityResult roleResult = await _userRepository.AssignUserToRole(adminUser, adminRole);
            if (!roleResult.Succeeded)
            {
                return BadRequest(roleResult.Errors);
            }
            return Ok();
        }

        /// <summary>
        /// Upload a profile picture
        /// </summary>
        [HttpPost]
        [Route("{id}/upload-avatar")]
        public async Task<ActionResult<User>> UploadAvatar(string id, IFormFile file)
        {
            User? entity = await _userRepository.GetOneOrDefault(id.ToString());

            if (entity == null)
                return NotFound();

            if (!CurrentUserHasRole(RoleConstants.ADMINISTRATOR))
            {
                if (GetCurrentUserId() != entity.Id)
                {
                    return Forbid();
                }
            }

            if (!CDNConstants.TYPES_IMAGES.Contains(file.ContentType.ToLower()))
            {
                return BadRequest(new ErrorResponse(new List<string> { ErrorCodes.FILETYPE_NOT_ALLOWED }));
            }

            CloudinaryDotNet.Actions.ImageUploadResult result = await _cloudinaryService.UploadImage(file, CDNConstants.DIRECTORY_PROFILE_PICTURES);

            entity.AvatarUri = result.Url.AbsoluteUri;
            await _userRepository.Update(entity);

            return Ok(entity);
        }
    }
}
