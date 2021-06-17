using System;
using System.Net;

namespace rest_api_blueprint.Exceptions
{
    public class GoogleAccessTokenRequestException : Exception
    {
        public readonly WebExceptionStatus StatusCode;

        public GoogleAccessTokenRequestException(
            string? message,
            WebExceptionStatus statusCode
        ) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
