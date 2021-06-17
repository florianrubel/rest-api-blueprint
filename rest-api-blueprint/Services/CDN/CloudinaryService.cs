using rest_api_blueprint.Models.CDN;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace rest_api_blueprint.Services.CDN
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly IOptions<CloudinaryOptions> _cloudinaryOptions;

        public CloudinaryService(IOptions<CloudinaryOptions> cloudinaryOptions)
        {
            _cloudinaryOptions = cloudinaryOptions;
        }

        public async Task<ImageUploadResult> UploadImage(IFormFile file, string targetDirectory)
        {
            string tempPath = System.IO.Path.GetTempFileName();

            Account account = new Account(
                _cloudinaryOptions.Value.CloudName,
                _cloudinaryOptions.Value.ApiKey,
                _cloudinaryOptions.Value.ApiSecret
            );

            Cloudinary cloudinary = new Cloudinary(account);

            using (System.IO.FileStream stream = System.IO.File.Create(tempPath))
            {
                await file.CopyToAsync(stream);
            }

            ImageUploadParams uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(tempPath),
                PublicId = $"{targetDirectory}/{file.FileName}",
                Overwrite = false
            };

            ImageUploadResult result = await cloudinary.UploadAsync(uploadParams);

            System.IO.File.Delete(tempPath);

            return result;
        }
    }
}
