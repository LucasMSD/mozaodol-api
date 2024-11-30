using MongoDB.Bson;
using ProjetoTelegram.Application.DTOs.ChatDTOs;
using ProjetoTelegram.Application.DTOs.MessageDTOs;
using ProjetoTelegram.Domain.Entities.ChatEntities;
using ProjetoTelegram.Domain.Entities.MessageEntities;

namespace ProjetoTelegram.Application.Interfaces.ChatInterfaces
{
    public interface IChatService
    {
        Task<ObjectId> CreateChat(CreateChatModel chatModel);
        // todo: refatorar
        Task<(MessageDto, IEnumerable<string>)> SendMessage(NewMessageModel newMessage);

        Task<IEnumerable<ChatDto>> GetAll(ObjectId userId);
        Task<IEnumerable<MessageDto>> GetMessages(ObjectId objectId, ObjectId chatId);
        Task SendNotifications(MessageDto messagem, Chat chat);
        // todo: refatorar
        Task<Message> SeenMessage(SeenMessageModel seenMessage);
    }
}
