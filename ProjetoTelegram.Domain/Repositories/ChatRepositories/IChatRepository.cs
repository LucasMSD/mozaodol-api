using MongoDB.Bson;
using ProjetoTelegram.Domain.Models.Chat;

namespace ProjetoTelegram.Domain.Repositories.ChatRepositories
{
    public interface IChatRepository
    {
        Task<Chat> Get(ObjectId chatId);
        Task<IEnumerable<Chat>> Get(IEnumerable<ObjectId> chatsIds);
        Task Insert(Chat chat);
    }
}
