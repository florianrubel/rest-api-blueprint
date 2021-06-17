using rest_api_blueprint.Models.Authentication.Google;
using System.Threading.Tasks;

namespace rest_api_blueprint.Services.Authentication
{
    public interface IGoogleOAuthService
    {
        Task<string> GetLoginUri(string ip);
        Task<GoogleUser> GetUser(GoogleGrantResponse googleGrantResponse);
    }
}