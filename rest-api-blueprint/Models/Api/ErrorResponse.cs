using System.Collections.Generic;

namespace rest_api_blueprint.Models.Api
{
    public class ErrorResponse
    {
        public ErrorResponse(List<string> errorCodes)
        {
            ErrorCodes = errorCodes;
        }

        public List<string> ErrorCodes { get; set; }
    }
}
