using System;

namespace rest_api_blueprint.Models.Social.Announcement
{
    public class ViewAnnouncement : UuidViewModel
    {
        public string Bodytext { get; set; }

        public string CreatorId { get; set; }

        public bool IsPublic { get; set; }

        public Guid? AddressId { get; set; }

        public string Title { get; set; }
    }
}
