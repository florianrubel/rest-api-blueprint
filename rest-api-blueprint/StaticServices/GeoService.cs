using NetTopologySuite.Geometries;

namespace rest_api_blueprint.StaticServices
{
    public static class GeoService
    {
        // read more: https://www.markopapic.com/finding-nearby-users-using-ef-core-spatial-data/
        public static Point ConvertLocationFromLatLng(double lat, double lng)
        {
            return new Point(lng, lat) { SRID = 4326, };
        }
    }
}
