using MongoDB.Bson;
using ProjetoTelegram.Domain.Entities.UserEntities;

namespace ProjetoTelegram.Domain.Repositories.UserRepositories
{
    public interface IUserRepository
    {
        public Task<User> Add(User user);

        public Task<bool> Exists(string usarName);
        Task<IEnumerable<User>> GetAll();
        public User GetByLogin(string username, string password);

        Task<User> Get(ObjectId _id);
        Task<IEnumerable<User>> Get(IEnumerable<ObjectId> ids);
        void UpdateContacts(User user);
        Task UpdatePushToken(ObjectId userId, string pushToken);
        Task AddToChat(IEnumerable<ObjectId> usersIds, ObjectId chatId);
    }
}
