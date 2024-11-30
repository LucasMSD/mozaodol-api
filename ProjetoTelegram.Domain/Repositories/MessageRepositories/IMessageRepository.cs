using MongoDB.Bson;
using ProjetoTelegram.Domain.Entities.MessageEntities;
using ProjetoTelegram.Domain.Enums;

namespace ProjetoTelegram.Domain.Repositories.MessageRepositories
{
    public interface IMessageRepository
    {
        Task<IEnumerable<Message>> GetByChat(ObjectId chatId);
        Task Insert(Message message);

        Task UpdateStatus(ObjectId _id, MessageStatus status);

        Task<Message> Get(ObjectId _id);
    }
}
