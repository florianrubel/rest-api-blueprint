using System;

namespace rest_api_blueprint.Models
{
    public abstract class UuidViewModel
    {
        public Guid Id { get; set; }

        public uint? OldId { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
