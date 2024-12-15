using FluentResults;
using MongoDB.Bson;
using Mozaodol.Domain.Entities.MessageEntities;

namespace Mozaodol.Domain.Services
{
    public interface IStorageService
    {
        Task<string> GetDownloadUrl(ObjectId storageId);
        Task<Result<ObjectId>> Upload(string contentBase64, MediaType type, ObjectId userId);
    }
}
