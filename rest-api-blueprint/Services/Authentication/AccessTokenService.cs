using rest_api_blueprint.Entities.Identity;
using rest_api_blueprint.Models.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;

namespace rest_api_blueprint.Services.Authentication
{
    public class AccessTokenService : BaseJwtService, IAccessTokenService
    {
        private readonly UserManager<User> _userManager;

        public AccessTokenService(IOptions<JwtTokenOptions> jwtTokenOptions, UserManager<User> userManager)
        {
            _userManager = userManager;
            _issuer = jwtTokenOptions.Value.Issuer;
            _audience = jwtTokenOptions.Value.Audience;
            _key = jwtTokenOptions.Value.Key;
            _bearerTTL = jwtTokenOptions.Value.BearerTTL;
        }

        protected override async Task EnrichPayload(JwtSecurityToken token, User user)
        {
            token.Payload.Add("avatarUri", user.AvatarUri);
            token.Payload.Add("roles", await _userManager.GetRolesAsync(user));
        }
    }
}
