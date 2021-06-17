using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using rest_api_blueprint.Constants;
using rest_api_blueprint.DbContexts;
using rest_api_blueprint.Entities.Social;
using rest_api_blueprint.Helpers;
using rest_api_blueprint.Models.Api;
using rest_api_blueprint.Models.Social.Announcement;
using rest_api_blueprint.StaticServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rest_api_blueprint.Repositories.Social
{
    public class AnnouncementRepository : UuidBaseRepository<Announcement>, IAnnouncementRepository<Announcement>
    {
        public AnnouncementRepository(MainDbContext context) : base(context)
        {

        }
        public virtual async Task<PagedList<Announcement>> GetMultiple(AnnouncementSearchParameters parameters)
        {
            IQueryable<Announcement> collection = _dbSet as IQueryable<Announcement>;

            if (parameters.Lat != null && parameters.Lng != null && parameters.RadiusInKm != null)
            {
                collection = collection.Include(x => x.Address);
                Point searchPoint = GeoService.ConvertLocationFromLatLng((double)parameters.Lat, (double)parameters.Lng);
                collection = collection.Where(r => r.Address != null && r.Address.Location != null && r.Address.Location.Distance(searchPoint) <= parameters.RadiusInKm * 1000);
            }

            if (parameters.IsPublic != null)
            {
                collection = collection.Where(r => r.IsPublic == parameters.IsPublic);
            }

            if (parameters.CreatedAtFrom != null)
            {
                collection = collection.Where(r => r.CreatedAt >= parameters.CreatedAtFrom);
            }

            if (parameters.CreatedAtTo != null)
            {
                collection = collection.Where(r => r.CreatedAt <= parameters.CreatedAtTo);
            }

            if (parameters.SearchQuery != null && parameters.SearchQuery.Length >= InputSizes.DEFAULT_TEXT_MIN_LENGTH)
            {
                collection = collection.Where(r =>
                    r.Title.Contains(parameters.SearchQuery, System.StringComparison.OrdinalIgnoreCase)
                    ||
                    r.Bodytext.Contains(parameters.SearchQuery, System.StringComparison.OrdinalIgnoreCase)
                );
            }

            collection = collection.ApplySort(parameters.OrderBy);

            PagedList<Announcement> pagedList = await PagedList<Announcement>.Create(collection, parameters.Page, parameters.PageSize);

            return pagedList;
        }
    }
}
