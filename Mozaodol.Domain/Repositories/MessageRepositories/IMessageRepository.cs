using FluentResults;
using MongoDB.Bson;
using Mozaodol.Domain.Entities.MessageEntities;
using Mozaodol.Domain.Enums;

namespace Mozaodol.Domain.Repositories.MessageRepositories
{
    public interface IMessageRepository
    {
        Task<Result<List<Message>>> GetByChat(ObjectId chatId);
        Task<Result> Insert(Message message);

        Task<Result> UpdateStatus(ObjectId _id, MessageStatus status);

        Task<Result<Message>> Get(ObjectId _id);
    }
}
