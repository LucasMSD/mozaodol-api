using MongoDB.Bson;
using Mozaodol.Domain.Entities.StorageEntities;

namespace Mozaodol.Domain.Repositories.StorageRepositories
{
    public interface IStorageRepository
    {
        Task Insert(Storage storage, ObjectId userId);
    }
}
