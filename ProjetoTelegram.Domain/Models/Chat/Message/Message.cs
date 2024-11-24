using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using ProjetoTelegram.Domain.Enums;

namespace ProjetoTelegram.Domain.Models.Chat.Message
{
    [BsonIgnoreExtraElements]
    public class Message
    {
        public ObjectId _id { get; set; }
        public string Text { get; set; }
        public ObjectId UserId { get; set; }
        public ObjectId ChatId { get; set; }
        public DateTime Timestamp { get; set; }
        public MessageStatus Status { get; set; }
        public string ExternalId { get; set; }
    }
}
