using MongoDB.Bson;

namespace ProjetoTelegram.Domain.Models.Chat
{
    public class NewMessageModel
    {
        public string Text { get; set; }
        public ObjectId UserId { get; set; }
        public ObjectId ChatId { get; set; }
        public string ExternalId { get; set; }
    }
}
