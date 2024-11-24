using MongoDB.Bson;

namespace ProjetoTelegram.Domain.Models.Chat
{
    public class SeenMessageModel
    {
        public ObjectId MessageId { get; set; }
    }
}
