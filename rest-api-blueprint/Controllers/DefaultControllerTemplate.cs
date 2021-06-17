using rest_api_blueprint.Constants.Identity;
using rest_api_blueprint.Models.Api;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace rest_api_blueprint.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public abstract class DefaultControllerTemplate : ControllerBase
    {
        protected virtual void SetPaginationHeaders(IPagedList pagedList)
        {
            Response.Headers.Add("Pagination.TotalCount", pagedList.TotalCount.ToString());
            Response.Headers.Add("Pagination.PageSize", pagedList.PageSize.ToString());
            Response.Headers.Add("Pagination.Page", pagedList.Page.ToString());
            Response.Headers.Add("Pagination.TotalPages", pagedList.TotalPages.ToString());
        }

        protected virtual bool CurrentUserHasRole(string role)
        {
            return HttpContext.User.IsInRole(role);
        }

        protected virtual string? GetCurrentUserId()
        {
            return (from claim in HttpContext.User.Claims where claim.Type == JwtClaims.ID select claim.Value).FirstOrDefault();
        }
    }
}
