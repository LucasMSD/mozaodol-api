using MongoDB.Bson;
using Mozaodol.Domain.Entities.UserEntities;

namespace Mozaodol.Domain.Repositories.UserRepositories
{
    public interface IUserRepository
    {
        Task<User> Add(User user);
        Task<bool> Exists(string usarName);
        Task<List<User>> GetAll();
        Task<User?> GetByLogin(string username, string password);
        Task<User?> Get(ObjectId _id);
        Task<List<User>> Get(IEnumerable<ObjectId> ids);
        Task UpdateContacts(ObjectId userId, IEnumerable<ObjectId> contacts);
        Task UpdatePushToken(ObjectId userId, string pushToken);
        Task AddToChat(IEnumerable<ObjectId> usersIds, ObjectId chatId);
    }
}
