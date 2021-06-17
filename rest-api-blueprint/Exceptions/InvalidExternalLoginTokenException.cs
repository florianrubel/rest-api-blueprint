using System;

namespace rest_api_blueprint.Exceptions
{
    public class InvalidExternalLoginTokenException : Exception
    {
        public readonly string? token;
        public InvalidExternalLoginTokenException(string? token, string? message) : base(message)
        {
            this.token = token;
        }
    }
}
