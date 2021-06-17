using rest_api_blueprint.Entities.Geo;
using rest_api_blueprint.Models.Api;
using rest_api_blueprint.Models.Geo.Address;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace rest_api_blueprint.Repositories.Geo
{
    public interface IAddressRepository<EntityType> : IUuidBaseRepository<EntityType> where EntityType : Address
    {
        Task UpdateWithoutGeocoding(Address entity);
        Task UpdateRangeWithoutGeocoding(IEnumerable<Address> entities);
        Task<PagedList<Address>> GetMultiple(AddressSearchParameters parameters);
    }
}
