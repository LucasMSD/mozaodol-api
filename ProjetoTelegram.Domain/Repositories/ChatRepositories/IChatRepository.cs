using FluentResults;
using MongoDB.Bson;
using ProjetoTelegram.Domain.Entities.ChatEntities;

namespace ProjetoTelegram.Domain.Repositories.ChatRepositories
{
    public interface IChatRepository
    {
        Task<Result<Chat>> Get(ObjectId chatId);
        Task<Result<List<Chat>>> Get(IEnumerable<ObjectId> chatsIds);
        Task<Result> Insert(Chat chat);
    }
}
