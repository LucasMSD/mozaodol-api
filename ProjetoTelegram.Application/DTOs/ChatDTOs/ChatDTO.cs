using MongoDB.Bson;

namespace ProjetoTelegram.Application.DTOs.ChatDTOs
{
    public class ChatDto
    {
        public ObjectId _id { get; set; }
        public string Name { get; set; }
    }
}
