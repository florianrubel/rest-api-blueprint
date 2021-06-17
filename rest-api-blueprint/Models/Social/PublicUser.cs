using System;

namespace rest_api_blueprint.Models.Social
{
    public class PublicUser : ExternalViewModel
    {
        public Guid Id { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset? LastLogin { get; set; }

        public bool LockoutEnabled { get; set; }


        public string? AboutMe { get; set; }

        public string? AvatarUri { get; set; }

        public DateTime? Birthday { get; set; }

        public string? FanOf { get; set; }

        public string FirstName { get; set; }

        public char? Gender { get; set; }
    }
}
