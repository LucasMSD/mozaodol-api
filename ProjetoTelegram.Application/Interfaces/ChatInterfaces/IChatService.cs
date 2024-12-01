using FluentResults;
using MongoDB.Bson;
using ProjetoTelegram.Application.DTOs.ChatDTOs;
using ProjetoTelegram.Application.DTOs.MessageDTOs;
using ProjetoTelegram.Domain.Entities.MessageEntities;

namespace ProjetoTelegram.Application.Interfaces.ChatInterfaces
{
    public interface IChatService
    {
        Task<Result<ObjectId>> CreateChat(CreateChatModel chatModel, string userId);
        // todo: refatorar
        Task<Result<MessageDto>> SendMessage(NewMessageModel newMessage);
        Task<Result<List<MessageDto>>> GetMessages(ObjectId objectId, ObjectId chatId);
        // todo: refatorar
        Task<Result<Message>> SeenMessage(SeenMessageModel seenMessage, string userId);
        Task OnOpenedChat(OnOpenedChatModel onOpenedChatModel, string userId);
        Task OnLeftChat(string userId);
        Task OnDisconnected(string userId, Exception? exception);
        Task OnConnected(string userId);
    }
}
