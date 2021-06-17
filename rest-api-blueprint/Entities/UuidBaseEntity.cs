using System;
using System.ComponentModel.DataAnnotations;

namespace rest_api_blueprint.Entities
{
    public abstract class UuidBaseEntity : BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
    }
}
