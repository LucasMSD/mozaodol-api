using MongoDB.Bson;
using ProjetoTelegram.Domain.Models.Chat;
using ProjetoTelegram.Domain.Models.Chat.Message;

namespace ProjetoTelegram.Domain.Services.ChatServices
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
