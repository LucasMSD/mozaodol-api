using MongoDB.Bson;

namespace ProjetoTelegram.Application.DTOs.ChatDTOs
{
    public class OnTypingDTO
    {
        public ObjectId ChatId { get; set; }
        public bool IsTypyng { get; set; }
    }
}
