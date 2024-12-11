using MongoDB.Bson;

namespace Mozaodol.Application.DTOs.MessageDTOs
{
    public class SendMessageDTO
    {
        public string Text { get; set; }
        public ObjectId ChatId { get; set; }
        public string ExternalId { get; set; }
    }
}
