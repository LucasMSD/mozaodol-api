using MongoDB.Bson;
using Mozaodol.Domain.Enums;

namespace Mozaodol.Application.DTOs.MessageDTOs
{
    public class MessageDto
    {
        public ObjectId _id { get; set; }
        public string Text { get; set; }
        public ObjectId UserId { get; set; }
        public string UserUsername { get; set; }
        public string Timestamp { get; set; }
        public ObjectId ChatId { get; set; }
        public MessageStatus Status { get; set; }
        public string ExternalId { get; set; }
    }
}
