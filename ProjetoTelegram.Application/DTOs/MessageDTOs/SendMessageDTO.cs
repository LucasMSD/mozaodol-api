using MongoDB.Bson;

namespace ProjetoTelegram.Application.DTOs.MessageDTOs
{
    public class SendMessageModel
    {
        public MessageDto Message { get; set; }
        public IEnumerable<ObjectId> UsersIds { get; set; }
    }
}
