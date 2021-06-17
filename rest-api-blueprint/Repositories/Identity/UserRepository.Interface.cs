using rest_api_blueprint.Entities.Identity;
using rest_api_blueprint.Models.Api;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace rest_api_blueprint.Repositories.Identity
{
    public interface IUserRepository
    {
        Task<IdentityResult> AssignUserToRole(User user, Role role);
        Task<IdentityResult> ConfirmEmail(User user, string token);
        Task<IdentityResult> Create(User user, string plainPassword);
        Task<string> GetEmailConfirmationToken(User user);
        Task<IEnumerable<User>> GetMultiple(IEnumerable<string> userIds, ShapingParameters parameters);
        Task<PagedList<User>> GetMultiple(SearchParameters parameters);
        Task<User> GetOneOrDefault(ClaimsPrincipal claimsPrincipal);
        Task<User> GetOneOrDefault(string id);
        Task<User> GetOneOrDefaultByName(string username);
        Task<User?> GetOneOrDefaultByFacebookUserId(string facebookUserId);
        Task<User?> GetOneOrDefaultByGoogleUserId(string googleUserId);
        Task<string> GetPasswordResetToken(User user);
        Task<IdentityResult> RemoveUserFromRole(User user, Role role);
        Task<IdentityResult> SetUserLockout(User user, bool lockout);
        Task<IdentityResult> Update(User user);
        Task<IdentityResult> ResetUserPassword(User user, string passwordResetToken, string plainPassword);
        Task<IEnumerable<string>> GetUserRoles(User user);
    }
}