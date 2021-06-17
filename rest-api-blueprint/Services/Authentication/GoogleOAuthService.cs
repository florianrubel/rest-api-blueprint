using rest_api_blueprint.Entities.Authentication;
using rest_api_blueprint.Exceptions;
using rest_api_blueprint.Models.Authentication.Google;
using rest_api_blueprint.Repositories.Authentication;
using rest_api_blueprint.StaticServices;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

// List of Scopes: https://developers.google.com/identity/protocols/oauth2/scopes#google-sign-in
// How To: https://developers.google.com/identity/protocols/oauth2/web-server

namespace rest_api_blueprint.Services.Authentication
{
    public class GoogleOAuthService : IGoogleOAuthService
    {
        private const string LOGIN_PROVIDER = "Google";

        private readonly IExternalLoginTokenRepository _externalLoginTokenRepository;

        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _redirectUri;

        private readonly bool _debug;

        private readonly List<string> _scopes = new List<string>{
            "email",
            "profile",
            "openid",
            "https://www.googleapis.com/auth/user.birthday.read",
            "https://www.googleapis.com/auth/user.gender.read"
        };

        public GoogleOAuthService(IOptions<GoogleOAuthOptions> options, IExternalLoginTokenRepository externalLoginTokenRepository)
        {
            _clientId = options.Value.ClientId;
            _clientSecret = options.Value.ClientKey;
            _redirectUri = options.Value.RedirectUri;
            _externalLoginTokenRepository = externalLoginTokenRepository;
            _debug = options.Value.Debug;
        }

        public async Task<string> GetLoginUri(string ip)
        {
            ExternalLoginToken externalLoginToken = await _externalLoginTokenRepository.GetNewExternalLoginToken(LOGIN_PROVIDER, ip, null);

            string base64Token = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(externalLoginToken.Token));
            return $"https://accounts.google.com/o/oauth2/v2/auth?scope={string.Join('+', _scopes)}&prompt=select_account&access_type=offline&include_granted_scopes=true&response_type=code&client_id={_clientId}&state={base64Token}&redirect_uri={_redirectUri}";
        }

        public async Task<GoogleUser> GetUser(GoogleGrantResponse googleGrantResponse)
        {
            string accessTokenUser = await GetAccessToken(googleGrantResponse);

            string uri = $"https://www.googleapis.com/oauth2/v1/userinfo?alt=json";
            WebRequest request = WebRequest.Create(uri);
            request.Headers.Add("Authorization", "Bearer " + accessTokenUser);

            WebResponse response = await request.GetResponseAsync();

            string responseBody = "";
            try
            {
                using (Stream dataStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(dataStream);
                    responseBody = reader.ReadToEnd();
                    if (_debug)
                        Console.WriteLine(responseBody);
                }
                response.Close();

                GoogleUserInfo googleUserInfo = JsonConvert.DeserializeObject<GoogleUserInfo>(responseBody, TextService.getSnakeCaseJsonSerializerSettings());
                GoogleMeResponse googleMeResponse = await GetAdditionalUserInfo(accessTokenUser);

                return new GoogleUser {
                    PublicProfile = googleUserInfo,
                    AdditionalRessources = googleMeResponse
                };
            }
            catch (WebException ex)
            {
                using (Stream responseStream = ex.Response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream);
                    responseBody = reader.ReadToEnd();
                    if (_debug)
                        Console.WriteLine(responseBody);
                }
                ex.Response.Close();
                throw new GoogleAccessTokenRequestException(responseBody, ex.Status);
            }
        }

        private async Task<GoogleMeResponse>GetAdditionalUserInfo(string accessToken)
        {
            string uri = $"https://people.googleapis.com/v1/people/me?personFields=genders,birthdays";
            WebRequest request = WebRequest.Create(uri);
            request.Headers.Add("Authorization", "Bearer " + accessToken);

            WebResponse response = await request.GetResponseAsync();

            string responseBody = "";
            try
            {
                using (Stream dataStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(dataStream);
                    responseBody = reader.ReadToEnd();
                    if (_debug)
                        Console.WriteLine(responseBody);
                }
                GoogleMeResponse googleMeResponse = JsonConvert.DeserializeObject<GoogleMeResponse>(responseBody);
                response.Close();
                return googleMeResponse;
            }
            catch (WebException ex)
            {
                using (Stream responseStream = ex.Response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream);
                    responseBody = reader.ReadToEnd();
                    if (_debug)
                        Console.WriteLine(responseBody);
                }
                ex.Response.Close();
                throw new GoogleAccessTokenRequestException(responseBody, ex.Status);
            }
        }

        private async Task<string> GetAccessToken(GoogleGrantResponse googleGrantResponse)
        {
            if (googleGrantResponse.State == null)
            {
                throw new InvalidExternalLoginTokenException(
                    googleGrantResponse.State,
                    "Invalid token has been used. Maybe malicious attempt."
                );
            }

            string decodedStateToken = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(googleGrantResponse.State));

            ExternalLoginToken? externalLoginToken = await _externalLoginTokenRepository.GetExternalLoginToken(LOGIN_PROVIDER, decodedStateToken, null);
            if (externalLoginToken == null)
            {
                throw new InvalidExternalLoginTokenException(
                    googleGrantResponse.State,
                    "Invalid token has been used. Maybe malicious attempt."
                );
            }

            GoogleAccessTokenRequest googleAccessTokenRequest = new GoogleAccessTokenRequest
            {
                ClientId = _clientId,
                ClientSecret = _clientSecret,
                Code = googleGrantResponse.Code,
                GrantType = "authorization_code",
                RedirectUri = _redirectUri
            };

            string accessTokenUri = $"https://oauth2.googleapis.com/token";
            WebRequest request = WebRequest.Create(accessTokenUri);
            request.Method = "POST";
            request.ContentType = "application/json";

            string payload = JsonConvert.SerializeObject(googleAccessTokenRequest, TextService.getSnakeCaseJsonSerializerSettings());
            byte[] payloadBytes = System.Text.Encoding.UTF8.GetBytes(payload);

            request.ContentLength = payloadBytes.Length;

            Stream requestStream = request.GetRequestStream();
            await requestStream.WriteAsync(payloadBytes, 0, payloadBytes.Length);
            requestStream.Close();

            string responseBody;

            try
            {
                WebResponse response = await request.GetResponseAsync();

                string statusDescription = ((HttpWebResponse)response).StatusDescription;
                HttpStatusCode statusCode = ((HttpWebResponse)response).StatusCode;

                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream);
                    responseBody = reader.ReadToEnd();
                }
                response.Close();

                GoogleAccessTokenResponse googleAccessTokenResponse = JsonConvert.DeserializeObject<GoogleAccessTokenResponse>(responseBody, TextService.getSnakeCaseJsonSerializerSettings());

                return googleAccessTokenResponse.AccessToken;
            }
            catch (WebException ex)
            {
                using (Stream responseStream = ex.Response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream);
                    responseBody = reader.ReadToEnd();
                    if (_debug)
                        Console.WriteLine(responseBody);
                }
                ex.Response.Close();
                throw new GoogleAccessTokenRequestException(responseBody, ex.Status);
            }
        }
    }
}
