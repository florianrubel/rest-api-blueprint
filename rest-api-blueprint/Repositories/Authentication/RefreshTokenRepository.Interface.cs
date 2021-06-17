using rest_api_blueprint.Entities.Authentication;
using rest_api_blueprint.Entities.Identity;
using rest_api_blueprint.Models.Authentication;
using System.Threading.Tasks;

namespace rest_api_blueprint.Repositories.Authentication
{
    public interface IRefreshTokenRepository
    {
        Task DeleteExpiredRefreshTokens(User user, bool saveChanges = true);
        Task DeleteRefreshToken(RefreshToken refreshToken);
        Task<RefreshToken> GetNewRefreshToken(User user, string ip);
        Task<RefreshToken> GetRefreshToken(RefreshTokenRequest requestRefreshToken, User user);
    }
}