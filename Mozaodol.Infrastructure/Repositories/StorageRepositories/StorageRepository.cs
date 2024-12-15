using MongoDB.Bson;
using Mozaodol.Domain.Entities.StorageEntities;
using Mozaodol.Domain.Repositories.StorageRepositories;
using Mozaodol.Infrastructure.Contexts.MongoDBContexts;

namespace Mozaodol.Infrastructure.Repositories.StorageRepositories
{
    public class StorageRepository : IStorageRepository
    {
        private readonly MongoDBContext _context;

        public StorageRepository(MongoDBContext dbContext)
        {
            _context = dbContext;
        }

        public async Task Insert(Storage storage, ObjectId userId)
        {
            await _context.Database.GetCollection<Storage>(nameof(Storage)).InsertOneAsync(storage);
        }
    }
}
