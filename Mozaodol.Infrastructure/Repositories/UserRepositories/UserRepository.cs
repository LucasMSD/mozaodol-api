using FluentResults;
using MongoDB.Bson;
using MongoDB.Driver;
using Mozaodol.Domain.Entities.UserEntities;
using Mozaodol.Domain.Repositories.UserRepositories;
using Mozaodol.Infrastructure.Contexts.MongoDBContexts;

namespace Mozaodol.Infrastructure.Repositories.UserRepositories
{
    public class UserRepository : IUserRepository
    {
        private readonly MongoDBContext _context;

        public UserRepository(MongoDBContext dbContext)
        {
            _context = dbContext;
        }

        public async Task<Result<User>> Add(User user)
        {
            await _context.Database.GetCollection<User>(nameof(User)).InsertOneAsync(user);

            return user;
        }

        public async Task<Result<bool>> Exists(string usarName)
        {
            var qtd = await _context.Database.GetCollection<User>(nameof(User)).CountDocumentsAsync(x => x.Username == usarName);
            return qtd > 0;
        }

        public async Task<Result<List<User>>> GetAll()
        {
            var result = await _context.Database.GetCollection<User>(nameof(User)).FindAsync(FilterDefinition<User>.Empty);
            return Result.Ok(await result.ToListAsync());
        }

        public async Task<Result<User>> Get(ObjectId _id)
        {
            return (await _context.Database.GetCollection<User>(nameof(User)).FindAsync(x => x._id == _id)).FirstOrDefault();
        }

        public async Task<Result<User>> GetByLogin(string username, string password)
        {
            return await _context.Database.GetCollection<User>(nameof(User)).FindSync(x => x.Username == username && x.Password == password).FirstOrDefaultAsync();
        }

        public async Task<Result<List<User>>> Get(IEnumerable<ObjectId> ids)
        {
            var result = await _context.Database.GetCollection<User>(nameof(User)).FindAsync(x => ids.Contains(x._id));
            return Result.Ok(await result.ToListAsync());
        }

        public async Task<Result> UpdateContacts(ObjectId userId, IEnumerable<ObjectId> contacts)
        {
            await _context.Database.GetCollection<User>(nameof(User)).UpdateOneAsync(x => x._id == userId,
                new UpdateDefinitionBuilder<User>().Set(x => x.Contacts, contacts));

            return Result.Ok();
        }

        public async Task<Result> UpdatePushToken(ObjectId userId, string pushToken)
        {
            await _context.Database.GetCollection<User>(nameof(User)).UpdateOneAsync(
                x => x._id == userId,
                new UpdateDefinitionBuilder<User>().Set(x => x.PushToken, pushToken));

            return Result.Ok();
        }

        public async Task<Result> AddToChat(IEnumerable<ObjectId> usersIds, ObjectId chatId)
        {
            var ids = usersIds.ToList();
            await _context.Database.GetCollection<User>(nameof(User)).UpdateManyAsync(
                x => ids.Contains(x._id),
                new UpdateDefinitionBuilder<User>().Push(x => x.ChatsIds, chatId));

            return Result.Ok();
        }
    }
}
