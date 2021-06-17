using rest_api_blueprint.Entities.Identity;
using System;
using System.Threading.Tasks;

namespace rest_api_blueprint.Services.Authentication
{
    public interface IBaseJwtService
    {
        Task<string> GetAccessToken(User user);
        Guid GetUserIdFromAccessToken(string accessToken);
    }
}