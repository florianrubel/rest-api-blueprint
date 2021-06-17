using rest_api_blueprint.Entities.Geo;
using rest_api_blueprint.Models.Authentication.Google;
using rest_api_blueprint.Models.Geo.Google;
using rest_api_blueprint.StaticServices;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;

namespace rest_api_blueprint.Services.Geo
{
    public class GoogleGeoService : IGoogleGeoService
    {
        private readonly IOptions<GoogleApiOptions> _googleApiOptions;

        public GoogleGeoService(IOptions<GoogleApiOptions> googleApiOptions)
        {
            _googleApiOptions = googleApiOptions;
        }

        public async Task<List<GeocodeResult>> GetGeoCodeResults(string searchString, string language)
        {
            string addressToGeocode = HttpUtility.UrlEncode($"{searchString}".ToLower());
            string uri = $"https://maps.googleapis.com/maps/api/geocode/json?key={_googleApiOptions.Value.ApiKey}&address={addressToGeocode}&language={language}";
            Console.WriteLine(uri);

            WebRequest request = WebRequest.Create(uri);
            WebResponse response = await request.GetResponseAsync();

            string responseBody;
            try
            {
                using (Stream dataStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(dataStream);
                    responseBody = reader.ReadToEnd();
                    if (_googleApiOptions.Value.Debug)
                        Console.WriteLine(responseBody);
                }
                response.Close();

                GeocodeResponse geocodeResponse = JsonConvert.DeserializeObject<GeocodeResponse>(responseBody, TextService.getSnakeCaseJsonSerializerSettings());

                foreach (GeocodeResult result in geocodeResponse.Results)
                {
                    if (result.PlaceId != null)
                    {
                        result.PlaceResult = await GetPlace(result.PlaceId, language);
                    }
                }

                return geocodeResponse.Results;

            }
            catch (WebException ex)
            {
                if (_googleApiOptions.Value.Debug) {
                    using (Stream responseStream = ex.Response.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(responseStream);
                        responseBody = reader.ReadToEnd();
                        Console.WriteLine(responseBody);
                    }
                    ex.Response.Close();
                }
            }
            return new List<GeocodeResult>();
        }

        public async Task GeocodeAddress(Address address)
        {
            string addressToGeocode = HttpUtility.UrlEncode($"{address.Street}, {address.PostalCode} {address.City}".ToLower());
            string uri = $"https://maps.googleapis.com/maps/api/geocode/json?key={_googleApiOptions.Value.ApiKey}&address={addressToGeocode}";
            if (_googleApiOptions.Value.Debug)
                Console.WriteLine(uri);

            WebRequest request = WebRequest.Create(uri);
            WebResponse response = await request.GetResponseAsync();

            string responseBody;
            try
            {
                using (Stream dataStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(dataStream);
                    responseBody = reader.ReadToEnd();
                    if (_googleApiOptions.Value.Debug)
                        Console.WriteLine(responseBody);
                }
                response.Close();

                GeocodeResponse geocodeResponse = JsonConvert.DeserializeObject<GeocodeResponse>(responseBody, TextService.getSnakeCaseJsonSerializerSettings());

                if (geocodeResponse.Results.Count > 0)
                {
                    try
                    {
                        LatLng latLng = geocodeResponse.Results.First().Geometry.Location;
                        address.Lat = latLng.Lat;
                        address.Lng = latLng.Lng;
                        address.Location = GeoService.ConvertLocationFromLatLng((double)address.Lat, (double)address.Lng);
                    }
                    catch
                    {
                        if (_googleApiOptions.Value.Debug)
                            Console.WriteLine($"Could not geocode the address: {addressToGeocode}");
                    }
                }

            }
            catch (WebException ex)
            {
                if (_googleApiOptions.Value.Debug) {
                    using (Stream responseStream = ex.Response.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(responseStream);
                        responseBody = reader.ReadToEnd();
                        Console.WriteLine(responseBody);
                    }
                    ex.Response.Close();
                }
            }
        }

        public async Task<PlaceResult?> GetPlace(string placeId, string language)
        {
            string uri = $"https://maps.googleapis.com/maps/api/place/details/json?key={_googleApiOptions.Value.ApiKey}&place_id={placeId}&language={language}";
            Console.WriteLine(uri);

            WebRequest request = WebRequest.Create(uri);
            WebResponse response = await request.GetResponseAsync();

            string responseBody;
            try
            {
                using (Stream dataStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(dataStream);
                    responseBody = reader.ReadToEnd();
                    if (_googleApiOptions.Value.Debug)
                        Console.WriteLine(responseBody);
                }
                response.Close();

                PlaceResponse placeResponse = JsonConvert.DeserializeObject<PlaceResponse>(responseBody, TextService.getSnakeCaseJsonSerializerSettings());

                return placeResponse.Result;

            }
            catch (WebException ex)
            {
                if (_googleApiOptions.Value.Debug) {
                    using (Stream responseStream = ex.Response.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(responseStream);
                        responseBody = reader.ReadToEnd();
                        Console.WriteLine(responseBody);
                    }
                    ex.Response.Close();
                }
            }
            return new PlaceResult();
        }
    }
}
