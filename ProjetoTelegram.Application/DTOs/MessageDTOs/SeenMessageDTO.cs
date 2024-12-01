using MongoDB.Bson;

namespace ProjetoTelegram.Application.DTOs.MessageDTOs
{
    public class SeenMessageDTO
    {
        public ObjectId MessageId { get; set; }
    }
}
