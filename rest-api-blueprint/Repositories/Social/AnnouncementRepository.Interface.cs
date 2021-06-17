using rest_api_blueprint.Entities.Social;
using rest_api_blueprint.Models.Api;
using rest_api_blueprint.Models.Social.Announcement;
using System.Threading.Tasks;

namespace rest_api_blueprint.Repositories.Social
{
    public interface IAnnouncementRepository<EntityType> : IUuidBaseRepository<EntityType> where EntityType : Announcement
    {
        Task<PagedList<Announcement>> GetMultiple(AnnouncementSearchParameters parameters);
    }
}
