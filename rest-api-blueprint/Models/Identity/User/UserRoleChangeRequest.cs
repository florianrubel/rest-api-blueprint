using System;
using System.ComponentModel.DataAnnotations;

namespace rest_api_blueprint.Models.Identity.User
{
    public class UserRoleChangeRequest
    {
        [Required]
        public Guid? RoleId { get; set; }
    }
}
