using rest_api_blueprint.Entities.Authentication;
using rest_api_blueprint.Entities.Identity;
using rest_api_blueprint.Models.Authentication;
using rest_api_blueprint.Models.Authentication.Facebook;
using rest_api_blueprint.Models.Authentication.Google;
using rest_api_blueprint.Models.Identity.User;
using rest_api_blueprint.Repositories.Authentication;
using rest_api_blueprint.Repositories.Identity;
using rest_api_blueprint.Services.Authentication;
using rest_api_blueprint.StaticServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rest_api_blueprint.Controllers.Authentication
{
    [Route("authentication/[controller]")]
    public class SignInController : DefaultControllerTemplate
    {
        private readonly SignInManager<User> _signInManager;
        private readonly IAccessTokenService _accessTokenService;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IUserRepository _userRepository;
        private readonly IFacebookLoginService _facebookLoginService;
        private readonly IGoogleOAuthService _googleOAuthService;

        public SignInController(
            SignInManager<User> signInManager,
            IUserRepository userRepository,
            IRefreshTokenRepository refreshTokenRepository,
            IAccessTokenService accessTokenService,
            IFacebookLoginService facebookLoginService,
            IGoogleOAuthService googleOAuthService
        )
        {
            _signInManager = signInManager;
            _refreshTokenRepository = refreshTokenRepository;
            _accessTokenService = accessTokenService;
            _userRepository = userRepository;
            _facebookLoginService = facebookLoginService;
            _googleOAuthService = googleOAuthService;
        }

        /// <summary>
        /// Sign in and receive all necessary authentication tokens.
        /// </summary>
        [AllowAnonymous]
        [HttpPost]
        [Route("")]
        public async Task<ActionResult<AuthenticationTokens>> SignIn([FromBody] SignInUser signInUser)
        {
            if (!ModelState.IsValid)
                return Unauthorized();

            Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(signInUser.UserName, signInUser.Password, false, false);


            if (!result.Succeeded)
                return Unauthorized();

            User? user = await _userRepository.GetOneOrDefaultByName(signInUser.UserName);

            if (user == null)
                return Unauthorized();

            if (user.LockoutEnabled)
                return Unauthorized();

            user.LastLogin = DateTimeOffset.UtcNow;
            await _userRepository.Update(user);

            string ip = HttpContext.Connection.RemoteIpAddress.ToString();

            RefreshToken refreshToken = await _refreshTokenRepository.GetNewRefreshToken(user, ip);

            return Ok(await GetTokenResponseObject(user, refreshToken));

        }

        /// <summary>
        /// Renew your authentication tokens.
        /// </summary>
        [AllowAnonymous]
        [HttpPost]
        [Route("refresh")]
        public async Task<ActionResult<AuthenticationTokens>> Refresh([FromBody] RefreshTokenRequest requestRefreshToken)
        {
            Guid userId;
            try
            {
                userId = _accessTokenService.GetUserIdFromAccessToken(requestRefreshToken.AccessToken);
            }
            catch
            {
                return Unauthorized();
            }

            User? user = await _userRepository.GetOneOrDefault(userId.ToString());

            if (user == null)
                return Unauthorized();

            if (user.LockoutEnabled)
                return Unauthorized();

            RefreshToken? refreshToken = await _refreshTokenRepository.GetRefreshToken(requestRefreshToken, user);

            if (refreshToken == null)
                return Unauthorized();

            if (refreshToken.ExpiresAt < DateTimeOffset.UtcNow)
            {
                string ip = HttpContext.Connection.RemoteIpAddress.ToString();
                refreshToken = await _refreshTokenRepository.GetNewRefreshToken(user, ip);
            }

            user.LastLogin = DateTimeOffset.UtcNow;
            await _userRepository.Update(user);

            return Ok(await GetTokenResponseObject(user, refreshToken));
        }


        /// <summary>
        /// Get Facebook login redirect url.
        /// </summary>
        [AllowAnonymous]
        [HttpGet]
        [Route("facebook-uri")]
        public async Task<ActionResult<FacebookLoginUriResponse>> GetFacebookUrl()
        {
            string ip = HttpContext.Connection.RemoteIpAddress.ToString();
            string loginUri = await _facebookLoginService.GetLoginUri(ip);
            return Ok(new FacebookLoginUriResponse { Uri = loginUri });
        }


        /// <summary>
        /// Sign in and receive all necessary authentication tokens via the Facebook Grant Response.
        /// </summary>
        [AllowAnonymous]
        [HttpPost]
        [Route("facebook")]
        public async Task<ActionResult<AuthenticationTokens>> SignInWithFacebook(FacebookGrantResponse facebookGrantResponse)
        {
            FacebookUserInfo facebookUser = await _facebookLoginService.GetUserInfo(facebookGrantResponse);

            User? user = await _userRepository.GetOneOrDefaultByFacebookUserId(facebookUser.Id);

            if (user == null)
            {
                user = await _userRepository.GetOneOrDefaultByName(facebookUser.Email);
            }

            if (user == null)
            {
                char gender = 'd';
                switch (facebookUser.Gender)
                {
                    case "male":
                        gender = 'm';
                        break;

                    case "female":
                        gender = 'f';
                        break;
                }

                user = new User
                {
                    UserName = facebookUser.Email,
                    Email = facebookUser.Email,
                    FacebookUserId = facebookUser.Id,
                    FirstName = facebookUser.FirstName,
                    LastName = facebookUser.LastName,
                    Gender = gender,
                    Birthday = facebookUser.Birthday,
                };

                IdentityResult identityResult = await _userRepository.Create(user, TextService.GeneratePassword());
                if (!identityResult.Succeeded)
                {
                    return BadRequest(identityResult.Errors);
                }

                user.EmailConfirmed = true;
                user.LockoutEnabled = false;
                identityResult = await _userRepository.Update(user);
                if (!identityResult.Succeeded)
                {
                    return BadRequest(identityResult.Errors);
                }
            }

            if (user.FacebookUserId == null)
            {
                user.FacebookUserId = facebookUser.Id;
                IdentityResult identityResult = await _userRepository.Update(user);
                if (!identityResult.Succeeded)
                {
                    return BadRequest(identityResult.Errors);
                }
            }

            if (user.LockoutEnabled)
                return Unauthorized();

            user.LastLogin = DateTimeOffset.UtcNow;
            await _userRepository.Update(user);

            string ip = HttpContext.Connection.RemoteIpAddress.ToString();

            RefreshToken refreshToken = await _refreshTokenRepository.GetNewRefreshToken(user, ip);

            return Ok(await GetTokenResponseObject(user, refreshToken));
        }


        /// <summary>
        /// Get Google login redirect url.
        /// </summary>
        [AllowAnonymous]
        [HttpGet]
        [Route("google-uri")]
        public async Task<ActionResult<GoogleLoginUriResponse>> GetGoogleUrl()
        {
            string ip = HttpContext.Connection.RemoteIpAddress.ToString();
            string loginUri = await _googleOAuthService.GetLoginUri(ip);
            return Ok(new GoogleLoginUriResponse { Uri = loginUri });
        }


        /// <summary>
        /// Sign in and receive all necessary authentication tokens via the Google Grant Response.
        /// </summary>
        [AllowAnonymous]
        [HttpPost]
        [Route("google")]
        public async Task<ActionResult> SignInWithGoogle(GoogleGrantResponse googleGrantResponse)
        {
            // TODO: Update gender and birthday if null for existing users
            GoogleUser googleUser = await _googleOAuthService.GetUser(googleGrantResponse);

            User? user = await _userRepository.GetOneOrDefaultByGoogleUserId(googleUser.PublicProfile.Id);

            if (user == null)
            {
                user = await _userRepository.GetOneOrDefaultByName(googleUser.PublicProfile.Email);
            }

            if (user == null)
            {
                user = new User
                {
                    UserName = googleUser.PublicProfile.Email,
                    Email = googleUser.PublicProfile.Email,
                    GoogleUserId = googleUser.PublicProfile.Id,
                    FirstName = googleUser.PublicProfile.GivenName,
                    LastName = googleUser.PublicProfile.FamilyName
                };


                List<GoogleMeResponseGender>? genders = googleUser.AdditionalRessources.Genders;
                if (genders != null && genders.Count > 0)
                {
                    GoogleMeResponseGender googleGender = genders.Where(g => g.Metadata.Primary).FirstOrDefault();
                    if (googleGender == null)
                    {
                        googleGender = genders.First();
                    }

                    char gender = 'd';
                    switch (googleGender.Value)
                    {
                        case "male":
                            gender = 'm';
                            break;

                        case "female":
                            gender = 'f';
                            break;
                    }

                    user.Gender = gender;
                }

                List<GoogleMeResponseBirthday> birthdays = googleUser.AdditionalRessources.Birthdays;
                if (birthdays != null && birthdays.Count > 0)
                {
                    GoogleMeResponseBirthday googleBirthday = birthdays.Where(g => g.Metadata.Primary).FirstOrDefault();
                    if (googleBirthday == null)
                    {
                        googleBirthday = birthdays.First();
                    }
                    user.Birthday = new DateTime(googleBirthday.Date.Year, googleBirthday.Date.Month, googleBirthday.Date.Day);
                }

                IdentityResult identityResult = await _userRepository.Create(user, TextService.GeneratePassword());
                if (!identityResult.Succeeded)
                {
                    return BadRequest(identityResult.Errors);
                }

                user.EmailConfirmed = true;
                user.LockoutEnabled = false;
                identityResult = await _userRepository.Update(user);
                if (!identityResult.Succeeded)
                {
                    return BadRequest(identityResult.Errors);
                }
            }

            if (user.GoogleUserId == null)
            {
                user.GoogleUserId = googleUser.PublicProfile.Id;
                IdentityResult identityResult = await _userRepository.Update(user);
                if (!identityResult.Succeeded)
                {
                    return BadRequest(identityResult.Errors);
                }
            }

            if (user.LockoutEnabled)
                return Unauthorized();

            user.LastLogin = DateTimeOffset.UtcNow;
            await _userRepository.Update(user);

            string ip = HttpContext.Connection.RemoteIpAddress.ToString();

            RefreshToken refreshToken = await _refreshTokenRepository.GetNewRefreshToken(user, ip);

            return Ok(await GetTokenResponseObject(user, refreshToken));
        }



        private async Task<AuthenticationTokens> GetTokenResponseObject(User user, RefreshToken refreshToken)
        {
            string accessToken = await _accessTokenService.GetAccessToken(user);

            return new AuthenticationTokens
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token
            };
        }
    }
}
