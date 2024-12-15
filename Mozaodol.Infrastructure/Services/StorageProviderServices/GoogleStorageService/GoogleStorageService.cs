using FluentResults;
using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Options;
using Mozaodol.Domain.Services;

namespace Mozaodol.Infrastructure.Services.StorageProviderServices.GoogleStorageService
{
    public class GoogleStorageService : IStorageProviderService
    {
        private readonly StorageClient _client;
        private readonly GoogleStorageSettings _googleStorageSettings;
        public GoogleStorageService(IOptions<GoogleStorageSettings> googleStorageSettings)
        {
            _client = StorageClient.Create();
            _googleStorageSettings = googleStorageSettings.Value;
        }

        public async Task<Result<string>> GetDownloadUrl(string name)
        {
            // todo: tratar exceções
            var signedUrl = await _client.CreateUrlSigner().SignAsync(
                _googleStorageSettings.BucketName,
                name,
                TimeSpan.FromMinutes(_googleStorageSettings.SignedUrlsExpirationTimeInSeconds));

            return Result.Ok(signedUrl);
        }

        public async Task<Result> Upload(Stream stream, string name, string extension)
        {
            // todo: ajustar contentType
            // todo tratar exceções
            await _client.UploadObjectAsync(_googleStorageSettings.BucketName, name, "", stream);
            return Result.Ok();
        }
    }
}
