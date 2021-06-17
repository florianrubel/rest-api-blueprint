using rest_api_blueprint.Entities;
using rest_api_blueprint.Models.Api;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace rest_api_blueprint.Repositories
{
    public interface IUuidBaseRepository<EntityType> where EntityType : UuidBaseEntity
    {
        Task<EntityType> Create(EntityType entity);
        Task<IEnumerable<EntityType>> CreateRange(IEnumerable<EntityType> entities);

        Task Update(EntityType entity);
        Task UpdateRange(IEnumerable<EntityType> entities);

        Task Delete(EntityType entity);
        Task DeleteRange(IEnumerable<EntityType> entities);

        Task<EntityType?> GetOneOrDefault(Guid id);

        Task<IEnumerable<EntityType>> GetMultiple(IEnumerable<Guid> ids, ShapingParameters parameters);
        Task<PagedList<EntityType>> GetMultiple(SearchParameters parameters);
    }
}