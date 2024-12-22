using MongoDB.Bson;
using Mozaodol.Domain.Entities.MessageEntities;

namespace Mozaodol.Application.DTOs.MessageDTOs
{
    public class SendMessageDTO
    {
        public string Text { get; set; }
        public ObjectId ChatId { get; set; }
        public SendMessageMediaDto Media { get; set; }
        public string ExternalId { get; set; }
    }

    public class SendMessageMediaDto
    {
        public MediaType Type { get; set; } = MediaType.Image;
        public string ContentBase64 { get; set; }
        public string Extension { get; set; } = "jpg";
    }
}
