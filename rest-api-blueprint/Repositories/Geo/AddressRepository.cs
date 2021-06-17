using rest_api_blueprint.Constants;
using rest_api_blueprint.DbContexts;
using rest_api_blueprint.Entities.Geo;
using rest_api_blueprint.Helpers;
using rest_api_blueprint.Models.Api;
using rest_api_blueprint.Models.Geo.Address;
using rest_api_blueprint.Services.Geo;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rest_api_blueprint.Repositories.Geo
{
    public class AddressRepository : UuidBaseRepository<Address>, IAddressRepository<Address>
    {
        private readonly IGoogleGeoService _googleGeoService;

        public AddressRepository(MainDbContext context, IGoogleGeoService googleGeoService) : base(context)
        {
            _googleGeoService = googleGeoService;
        }

        public override async Task<Address> Create(Address entity)
        {
            await _googleGeoService.GeocodeAddress(entity);
            return await base.Create(entity);
        }

        public override async Task<IEnumerable<Address>> CreateRange(IEnumerable<Address> entities)
        {
            foreach (Address entity in entities)
            {
                await _googleGeoService.GeocodeAddress(entity);
            }
            return await base.CreateRange(entities);
        }

        public override async Task Update(Address entity)
        {
            await _googleGeoService.GeocodeAddress(entity);
            await base.Update(entity);
        }

        public override async Task UpdateRange(IEnumerable<Address> entities)
        {
            foreach (Address entity in entities)
            {
                await _googleGeoService.GeocodeAddress(entity);
            }
            await base.UpdateRange(entities);
        }

        public async Task UpdateWithoutGeocoding(Address entity)
        {
            await base.Update(entity);
        }

        public async Task UpdateRangeWithoutGeocoding(IEnumerable<Address> entities)
        {
            await base.UpdateRange(entities);
        }

        public virtual async Task<PagedList<Address>> GetMultiple(AddressSearchParameters parameters)
        {
            IQueryable<Address> collection = _dbSet as IQueryable<Address>;

            if (parameters.UserIds != null)
            {
                List<string> userIds = parameters.UserIds.Split(',').ToList();
                if (userIds.Count > 0)
                {
                    collection = collection.Where(r => userIds.Contains(r.UserId));
                }
            }

            if (parameters.SearchQuery != null && parameters.SearchQuery.Length >= InputSizes.DEFAULT_TEXT_MIN_LENGTH)
            {
                collection = collection.Where(r =>
                    r.City.Contains(parameters.SearchQuery, System.StringComparison.OrdinalIgnoreCase)
                    ||
                    (r.Company != null && r.Company.Contains(parameters.SearchQuery, System.StringComparison.OrdinalIgnoreCase))
                    ||
                    r.FirstName.Contains(parameters.SearchQuery, System.StringComparison.OrdinalIgnoreCase)
                    ||
                    r.LastName.Contains(parameters.SearchQuery, System.StringComparison.OrdinalIgnoreCase)
                    ||
                    r.PostalCode.Contains(parameters.SearchQuery, System.StringComparison.OrdinalIgnoreCase)
                    ||
                    r.Street.Contains(parameters.SearchQuery, System.StringComparison.OrdinalIgnoreCase)
                );
            }

            collection = collection.ApplySort(parameters.OrderBy);

            PagedList<Address> pagedList = await PagedList<Address>.Create(collection, parameters.Page, parameters.PageSize);

            return pagedList;
        }
    }
}
