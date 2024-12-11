using FluentResults;
using MongoDB.Bson;
using Mozaodol.Domain.Entities.UserEntities;

namespace Mozaodol.Domain.Repositories.UserRepositories
{
    public interface IUserRepository
    {
        Task<Result<User>> Add(User user);
        Task<Result<bool>> Exists(string usarName);
        Task<Result<List<User>>> GetAll();
        Task<Result<User>> GetByLogin(string username, string password);
        Task<Result<User>> Get(ObjectId _id);
        Task<Result<List<User>>> Get(IEnumerable<ObjectId> ids);
        Task<Result> UpdateContacts(ObjectId userId, IEnumerable<ObjectId> contacts);
        Task<Result> UpdatePushToken(ObjectId userId, string pushToken);
        Task<Result> AddToChat(IEnumerable<ObjectId> usersIds, ObjectId chatId);
    }
}
