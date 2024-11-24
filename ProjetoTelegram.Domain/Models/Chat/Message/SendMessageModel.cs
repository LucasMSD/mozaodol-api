using MongoDB.Bson;

namespace ProjetoTelegram.Domain.Models.Chat.Message
{
    public class SendMessageModel
    {
        public MessageDto Message { get; set; }
        public IEnumerable<ObjectId> UsersIds { get; set; }


        
    }
}
