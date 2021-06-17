using rest_api_blueprint.Entities.Authentication;
using System.Threading.Tasks;

namespace rest_api_blueprint.Repositories.Authentication
{
    public interface IExternalLoginTokenRepository
    {
        Task DeleteExpiredExternalLoginTokens(string? userId, bool saveChanges);
        Task<ExternalLoginToken?> GetExternalLoginToken(string loginProvider, string token, string? userId);
        Task<ExternalLoginToken> GetNewExternalLoginToken(string loginProvider, string ip, string? userId);
    }
}