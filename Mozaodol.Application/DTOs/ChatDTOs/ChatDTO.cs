using MongoDB.Bson;

namespace Mozaodol.Application.DTOs.ChatDTOs
{
    public class ChatDto
    {
        public ObjectId _id { get; set; }
        public string Name { get; set; }
    }
}
