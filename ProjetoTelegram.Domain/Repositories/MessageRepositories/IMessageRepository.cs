using FluentResults;
using MongoDB.Bson;
using ProjetoTelegram.Domain.Entities.MessageEntities;
using ProjetoTelegram.Domain.Enums;

namespace ProjetoTelegram.Domain.Repositories.MessageRepositories
{
    public interface IMessageRepository
    {
        Task<Result<List<Message>>> GetByChat(ObjectId chatId);
        Task<Result> Insert(Message message);

        Task<Result> UpdateStatus(ObjectId _id, MessageStatus status);

        Task<Result<Message>> Get(ObjectId _id);
    }
}
