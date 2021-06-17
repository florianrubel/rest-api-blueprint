using Microsoft.AspNetCore.Identity;
using System;

namespace rest_api_blueprint.Entities.Identity
{
    public class Role : IdentityRole
    {
        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
