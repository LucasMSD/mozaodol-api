using MongoDB.Bson;
using Mozaodol.Domain.Entities.ChatEntities;

namespace Mozaodol.Domain.Repositories.ChatRepositories
{
    public interface IChatRepository
    {
        Task<Chat?> Get(ObjectId chatId);
        Task<List<Chat>> Get(IEnumerable<ObjectId> chatsIds);
        Task Insert(Chat chat);
    }
}
