using rest_api_blueprint.Entities.Authentication;
using rest_api_blueprint.Exceptions;
using rest_api_blueprint.Models.Authentication.Facebook;
using rest_api_blueprint.Repositories.Authentication;
using rest_api_blueprint.StaticServices;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

// https://developers.facebook.com/docs/facebook-login/manually-build-a-login-flow
namespace rest_api_blueprint.Services.Authentication
{
    public class FacebookLoginService : IFacebookLoginService
    {
        private const string LOGIN_PROVIDER = "Facebook";

        private readonly IExternalLoginTokenRepository _externalLoginTokenRepository;

        private readonly string _appId;
        private readonly string _appSecret;
        private readonly string _redirectUri;

        private readonly bool _debug;

        private readonly List<string> _scopes = new List<string>{
            "email",
            "user_gender",
            "user_birthday"
        };

        private readonly List<string> _fields = new List<string>
        {
            "id",
            "name",
            "first_name",
            "last_name",
            "birthday",
            "gender",
            "email",
            "picture"
        };

        public FacebookLoginService(IOptions<FacebookOptions> options, IExternalLoginTokenRepository externalLoginTokenRepository)
        {
            _externalLoginTokenRepository = externalLoginTokenRepository;
            _appId = options.Value.AppId;
            _appSecret = options.Value.AppSecret;
            _redirectUri = options.Value.RedirectUri;
            _debug = options.Value.Debug;
        }

        public async Task<string> GetLoginUri(string ip)
        {
            ExternalLoginToken externalLoginToken = await _externalLoginTokenRepository.GetNewExternalLoginToken(LOGIN_PROVIDER, ip, null);
            string base64Token = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(externalLoginToken.Token));

            return $"https://www.facebook.com/v9.0/dialog/oauth?client_id={_appId}&redirect_uri={_redirectUri}&state={base64Token}&scope={string.Join(',',_scopes)}";
        }

        public async Task<FacebookUserInfo> GetUserInfo(FacebookGrantResponse facebookGrantResponse)
        {
            string accessTokenUser = await GetAccessToken(facebookGrantResponse);
            string accessTokenApp = await GetAppAccessToken();

            FacebookAccessTokenContent accessTokenContent = await GetAccessTokenContent(accessTokenUser, accessTokenApp);

            string uri = $"https://graph.facebook.com/v9.0/{accessTokenContent.Data.UserId}?fields={string.Join(',',_fields)}";
            WebRequest request = WebRequest.Create(uri);
            request.Headers.Add("Authorization", "Bearer " + accessTokenUser);

            string responseBody;
            try
            {
                WebResponse response = await request.GetResponseAsync();
                using (Stream dataStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(dataStream);
                    responseBody = reader.ReadToEnd();
                    if (_debug)
                        Console.WriteLine(responseBody);
                }
                response.Close();

                FacebookUserInfo facebookUserInfo = JsonConvert.DeserializeObject<FacebookUserInfo>(responseBody, TextService.getSnakeCaseJsonSerializerSettings());
                return facebookUserInfo;
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
                throw new FacebookAccessTokenRequestException(responseBody, ex.Status);
            }
        }

        private async Task<FacebookAccessTokenContent> GetAccessTokenContent(string accessToken, string appAccessToken)
        {
            string accessTokenUri = $"https://graph.facebook.com/debug_token?input_token={accessToken}&access_token={appAccessToken}";
            WebRequest request = WebRequest.Create(accessTokenUri);

            string responseBody;

            try
            {
                WebResponse response = await request.GetResponseAsync();
                using (Stream dataStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(dataStream);
                    responseBody = reader.ReadToEnd();
                    if (_debug)
                        Console.WriteLine(responseBody);
                }
                response.Close();

                FacebookAccessTokenContent facebookAccessTokenContent = JsonConvert.DeserializeObject<FacebookAccessTokenContent>(responseBody, TextService.getSnakeCaseJsonSerializerSettings());
                return facebookAccessTokenContent;
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
                throw new FacebookAccessTokenRequestException(responseBody, ex.Status);
            }
        }

        private async Task<string> GetAccessToken(FacebookGrantResponse facebookGrantResponse)
        {
            if (facebookGrantResponse.State == null)
            {
                throw new InvalidExternalLoginTokenException(
                    facebookGrantResponse.State,
                    "Invalid token has been used. Maybe malicious attempt."
                );
            }

            string decodedStateToken = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(facebookGrantResponse.State));

            ExternalLoginToken? externalLoginToken = await _externalLoginTokenRepository.GetExternalLoginToken(LOGIN_PROVIDER, decodedStateToken, null);
            if(externalLoginToken == null)
            {
                throw new InvalidExternalLoginTokenException(
                    facebookGrantResponse.State,
                    "Invalid token has been used. Maybe malicious attempt."
                );
            }

            string accessTokenUri = $"https://graph.facebook.com/v9.0/oauth/access_token?client_id={_appId}&redirect_uri={_redirectUri}&client_secret={_appSecret}&code={facebookGrantResponse.Code}";
            WebRequest request = WebRequest.Create(accessTokenUri);

            string responseBody;
            try
            {
                WebResponse response = await request.GetResponseAsync();
                using (Stream dataStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(dataStream);
                    responseBody = reader.ReadToEnd();
                    if (_debug)
                        Console.WriteLine(responseBody);
                }
                response.Close();

                FacebookAccessTokenResponse facebookAccessTokenResponse = JsonConvert.DeserializeObject<FacebookAccessTokenResponse>(responseBody, TextService.getSnakeCaseJsonSerializerSettings());

                return facebookAccessTokenResponse.AccessToken;
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
                throw new FacebookAccessTokenRequestException(responseBody, ex.Status);
            }
        }

        private async Task<string> GetAppAccessToken()
        {
            string accessTokenUri = $"https://graph.facebook.com/oauth/access_token?client_id={_appId}&client_secret={_appSecret}&grant_type=client_credentials";
            WebRequest request = WebRequest.Create(accessTokenUri);

            string responseBody;
            try
            {
                WebResponse response = await request.GetResponseAsync();
                using (Stream dataStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(dataStream);
                    responseBody = reader.ReadToEnd();
                    if (_debug)
                        Console.WriteLine(responseBody);
                }
                response.Close();

                FacebookAccessTokenResponse facebookAccessTokenResponse = JsonConvert.DeserializeObject<FacebookAccessTokenResponse>(responseBody, TextService.getSnakeCaseJsonSerializerSettings());
                return facebookAccessTokenResponse.AccessToken;
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
                throw new FacebookAccessTokenRequestException(responseBody, ex.Status);
            }
        }
    }
}
