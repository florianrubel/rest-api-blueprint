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
    public class RestoreController : DefaultControllerTemplate
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;


        public RestoreController(
            IUserRepository userRepository,
            IEmailService emailService
        )
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _emailService = emailService;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("request-password-reset")]
        public async Task<IActionResult> RequestPasswordReset([FromBody] ResetPasswordEmailRequest resetPasswordEmailRequest)
        {
            User user = await _userRepository.GetOneOrDefaultByName(resetPasswordEmailRequest.UserName);
            if (user == null || !user.EmailConfirmed)
                return BadRequest();

            string token = await _userRepository.GetPasswordResetToken(user);
            string base64Token = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(token));

            await _emailService.SendResetPasswordMail(user.UserName, user.Email, base64Token);

            return Ok();
        }

        [AllowAnonymous]
        [HttpPatch]
        [Route("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest resetPasswordRequest)
        {
            User user = await _userRepository.GetOneOrDefaultByName(resetPasswordRequest.UserName);
            if (user == null)
                return BadRequest();

            byte[] tokenBytes = Convert.FromBase64String(resetPasswordRequest.Token);

            IdentityResult result = await _userRepository.ResetUserPassword(user, System.Text.Encoding.UTF8.GetString(tokenBytes), resetPasswordRequest.Password);

            if (result.Succeeded)
                return Ok();

            return BadRequest(result.Errors);
        }
    }
}
