using rest_api_blueprint.Entities.Identity;
using rest_api_blueprint.Models.Identity.User;
using rest_api_blueprint.Repositories.Identity;
using rest_api_blueprint.Services.Email;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace rest_api_blueprint.Controllers.Authentication
{
    [Route("authentication/[controller]")]
    public class SignUpController : DefaultControllerTemplate
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;


        public SignUpController(
            IUserRepository userRepository,
            IMapper mapper,
            IEmailService emailService
        )
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _mapper = mapper;
            _emailService = emailService;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("")]
        public async Task<IActionResult> SignUp([FromBody] SignUpUser signUpUser)
        {
            User user = _mapper.Map<User>(signUpUser);

            // UserName is handled in the UserProfile.
            IdentityResult result = await _userRepository.Create(user, signUpUser.Password);
            if (result.Succeeded)
            {
                string token = await _userRepository.GetEmailConfirmationToken(user);
                string base64Token = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(token));
                await _emailService.SendEmailConfirmationMail(user.Id, user.UserName, user.Email, base64Token);
                return Ok();
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }

        [AllowAnonymous]
        [HttpPatch]
        [Route("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromBody] EmailConfirmationRequest emailConfirmationRequest)
        {
            User user = await _userRepository.GetOneOrDefaultByName(emailConfirmationRequest.UserName);
            if (user == null)
            {
                return NotFound();
            }

            if (user.EmailConfirmed == true)
                return Forbid();

            byte[] tokenBytes = Convert.FromBase64String(emailConfirmationRequest.Token);
            IdentityResult result = await _userRepository.ConfirmEmail(user, System.Text.Encoding.UTF8.GetString(tokenBytes));

            if (result.Succeeded)
            {
                IdentityResult identityResult = await _userRepository.SetUserLockout(user, false);
                if (identityResult.Succeeded)
                {
                    await _emailService.SendAccountActivatedMail(user.UserName, user.Email);

                    return Ok();
                }

                return BadRequest(identityResult.Errors);
            }
            return BadRequest(result.Errors);
        }
    }
}
