using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace rest_api_blueprint.Services.CDN
{
    public interface ICloudinaryService
    {
        Task<ImageUploadResult> UploadImage(IFormFile file, string targetDirectory);
    }
}