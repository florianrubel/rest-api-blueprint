using rest_api_blueprint.Models.Authentication.Facebook;
using System.Threading.Tasks;

namespace rest_api_blueprint.Services.Authentication
{
    public interface IFacebookLoginService
    {
        Task<string> GetLoginUri(string ip);
        Task<FacebookUserInfo> GetUserInfo(FacebookGrantResponse facebookGrantResponse);

    }
}
