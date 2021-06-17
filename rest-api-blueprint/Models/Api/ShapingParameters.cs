namespace rest_api_blueprint.Models.Api
{
    public class ShapingParameters
    {
        public string OrderBy { get; set; } = "CreatedAt desc";

        public string Fields { get; set; } = "";
    }
}
