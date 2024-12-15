namespace Mozaodol.Infrastructure.Services.StorageProviderServices.GoogleStorageService
{
    public class GoogleStorageSettings
    {
        public string BucketName { get; set; }
        public int SignedUrlsExpirationTimeInSeconds { get; set; } = 60;
    }
}
