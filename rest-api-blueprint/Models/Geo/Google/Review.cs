namespace rest_api_blueprint.Models.Geo.Google
{
    public class Review
    {
        public string? AuthorName { get; set; }

        public string? AuthorUrl { get; set; }

        public string? Language { get; set; }

        public string? ProfilePhotoUrl { get; set; }

        public double? Rating { get; set; }

        public string? RelativeTimeDescription { get; set; }

        public string? Text { get; set; }

        public int? Time { get; set; }
    }
}
