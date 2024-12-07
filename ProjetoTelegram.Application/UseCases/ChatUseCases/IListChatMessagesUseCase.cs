using MongoDB.Bson;
using ProjetoTelegram.Application.DTOs.MessageDTOs;

namespace ProjetoTelegram.Application.UseCases.ChatUseCases
{
    public interface IListChatMessagesUseCase : IUseCase<ObjectId, List<MessageDto>>
    {
    }
}
