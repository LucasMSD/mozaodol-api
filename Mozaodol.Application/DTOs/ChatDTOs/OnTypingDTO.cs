using MongoDB.Bson;

namespace Mozaodol.Application.DTOs.ChatDTOs
{
    public class OnTypingDTO
    {
        public ObjectId ChatId { get; set; }
        public bool IsTyping { get; set; }
    }
}
