using MongoDB.Bson;

namespace ProjetoTelegram.Application.DTOs.ChatDTOs
{
    public class SeenMessageModel
    {
        public ObjectId MessageId { get; set; }
    }
}
