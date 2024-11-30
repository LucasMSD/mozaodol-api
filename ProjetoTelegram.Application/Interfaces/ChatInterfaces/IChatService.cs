using FluentResults;
using MongoDB.Bson;
using ProjetoTelegram.Application.DTOs.ChatDTOs;
using ProjetoTelegram.Application.DTOs.MessageDTOs;
using ProjetoTelegram.Domain.Entities.ChatEntities;
using ProjetoTelegram.Domain.Entities.MessageEntities;

namespace ProjetoTelegram.Application.Interfaces.ChatInterfaces
{
    public interface IChatService
    {
        Task<Result<ObjectId>> CreateChat(CreateChatModel chatModel);
        // todo: refatorar
        Task<Result<(MessageDto, List<string>)>> SendMessage(NewMessageModel newMessage);
        Task<Result<List<ChatDto>>> GetAll(ObjectId userId);
        Task<Result<List<MessageDto>>> GetMessages(ObjectId objectId, ObjectId chatId);
        Task<Result> SendNotifications(MessageDto messagem, Chat chat);
        // todo: refatorar
        Task<Result<Message>> SeenMessage(SeenMessageModel seenMessage);
    }
}
