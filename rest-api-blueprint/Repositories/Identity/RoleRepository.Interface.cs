using rest_api_blueprint.Entities.Identity;
using rest_api_blueprint.Models.Api;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace rest_api_blueprint.Repositories.Identity
{
    public interface IRoleRepository
    {
        Task<IdentityResult> Create(Role role);
        Task<IdentityResult> Delete(Role role);
        Task<IEnumerable<Role>> GetMultiple(IEnumerable<string> roleIds, ShapingParameters parameters);
        Task<PagedList<Role>> GetMultiple(SearchParameters parameters);
        Task<IEnumerable<Role>> GetMultipleByNames(IEnumerable<string> names, ShapingParameters parameters);
        Task<Role> GetOneOrDefault(string id);
        Task<Role> GetOneOrDefaultByMame(string name);
        Task<bool> RoleExists(string roleId);
        Task<IdentityResult> Update(Role role);
    }
}