using MongoDB.Bson;
using Mozaodol.Domain.Entities.MessageEntities;
using Mozaodol.Domain.Enums;

namespace Mozaodol.Domain.Repositories.MessageRepositories
{
    public interface IMessageRepository
    {
        Task<List<Message>> GetByChat(ObjectId chatId);
        Task Insert(Message message);

        Task UpdateStatus(ObjectId _id, MessageStatus status);

        Task<Message> Get(ObjectId _id);
    }
}
