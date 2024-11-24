using MongoDB.Bson;
using MongoDB.Driver;
using ProjetoTelegram.Domain.Models.User;

namespace ProjetoTelegram.Domain.Repositories.UserRepositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DbContext _context;

        public UserRepository(DbContext dbContext)
        {
            _context = dbContext;
        }

        public async Task<User> Add(User user)
        {
            await _context.Database.GetCollection<User>(nameof(User)).InsertOneAsync(user);

            return user;
        }

        public async Task<bool> Exists(string usarName)
        {
            var qtd = await _context.Database.GetCollection<User>(nameof(User)).CountDocumentsAsync(x => x.Username == usarName);
            return qtd > 0;
        }

        public async Task<IEnumerable<User>> GetAll()
        {
            return (await _context.Database.GetCollection<User>(nameof(User)).FindAsync(FilterDefinition<User>.Empty)).ToEnumerable();
        }

        public async Task<User> Get(ObjectId _id)
        {
            return (await _context.Database.GetCollection<User>(nameof(User)).FindAsync(x => x._id == _id)).FirstOrDefault();
        }

        public User GetByLogin(string username, string password)
        {
            return _context.Database.GetCollection<User>(nameof(User)).FindSync(x => x.Username == username && x.Password == password).FirstOrDefault();
        }

        public async Task<IEnumerable<User>> Get(IEnumerable<ObjectId> ids)
        {
            return (await _context.Database.GetCollection<User>(nameof(User)).FindAsync(x => ids.Contains(x._id))).ToEnumerable();
        }

        public void UpdateContacts(User user)
        {
            _context.Database.GetCollection<User>(nameof(User)).UpdateOne(x => x._id == user._id,
                new UpdateDefinitionBuilder<User>().Set(x => x.Contacts, user.Contacts));
        }

        public async Task UpdatePushToken(ObjectId userId, UpdatePushTokenModel updatePushTokenModel)
        {
            await _context.Database.GetCollection<User>(nameof(User)).UpdateOneAsync(
                x => x._id == userId,
                new UpdateDefinitionBuilder<User>().Set(x => x.PushToken, updatePushTokenModel.PushToken));
        }

        public async Task AddToChat(IEnumerable<ObjectId> usersIds, ObjectId chatId)
        {
            var ids = usersIds.ToList();
            await _context.Database.GetCollection<User>(nameof(User)).UpdateManyAsync(
                x => ids.Contains(x._id),
                new UpdateDefinitionBuilder<User>().Push(x => x.ChatsIds, chatId));
        }
    }
}
