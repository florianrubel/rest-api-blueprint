using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace rest_api_blueprint.Entities
{
    public abstract class BaseEntity
    {
        public uint? OldId { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
