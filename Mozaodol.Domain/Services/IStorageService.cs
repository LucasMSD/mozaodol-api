using FluentResults;
using MongoDB.Bson;
using Mozaodol.Domain.Entities.MessageEntities;

namespace Mozaodol.Domain.Services
{
    public interface IStorageService
    {
        Task<Result<string>> GetDownloadUrl(ObjectId storageId);
        Task<Result<ObjectId>> Upload(string contentBase64, string extension, MediaType type, ObjectId userId);
    }
}
