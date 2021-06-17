using System;

namespace rest_api_blueprint.Models.Identity.Role
{
    public class ViewRole : ExternalViewModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
