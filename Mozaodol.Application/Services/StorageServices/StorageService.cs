using FluentResults;
using MongoDB.Bson;
using Mozaodol.Domain.Entities.MessageEntities;
using Mozaodol.Domain.Entities.StorageEntities;
using Mozaodol.Domain.Repositories.StorageRepositories;
using Mozaodol.Domain.Services;

namespace Mozaodol.Application.Services.StorageServices
{
    public class StorageService : IStorageService
    {
        private readonly IStorageProviderService _storageProvider;
        private readonly IStorageRepository _storageRepository;

        public StorageService(
            IStorageProviderService storageProvider,
            IStorageRepository storageRepository)
        {
            _storageProvider = storageProvider;
            _storageRepository = storageRepository;
        }

        public async Task<Result<string>> GetDownloadUrl(ObjectId storageId)
        {
            var result = await _storageProvider.GetDownloadUrl(storageId.ToString());
            if (result.IsFailed)
                return Result.Fail(result.Errors);

            return result;
        }

        public async Task<Result<ObjectId>> Upload(string contentBase64, string extension, MediaType type, ObjectId userId)
        {
            var storage = new Storage
            {
                Provider = CloudProvider.Gcp,
                Type = type,
                UserId = userId,
                Extension = extension
            };

            await _storageRepository.Insert(storage, userId);

            var stream = new MemoryStream(Convert.FromBase64String(contentBase64));
            var uploadResult = await _storageProvider.Upload(stream, $"{userId}/{storage._id}", extension);
            if (uploadResult.IsFailed)
                return Result.Fail(uploadResult.Errors);

            return storage._id;
        }
    }
}
