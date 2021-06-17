using System;
using System.Net;

namespace rest_api_blueprint.Exceptions
{
    public class FacebookAccessTokenRequestException : Exception
    {
        public readonly WebExceptionStatus StatusCode;

        public FacebookAccessTokenRequestException(
            string? message,
            WebExceptionStatus statusCode
        ) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
