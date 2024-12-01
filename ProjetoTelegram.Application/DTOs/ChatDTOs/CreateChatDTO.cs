using MongoDB.Bson;

namespace ProjetoTelegram.Application.DTOs.ChatDTOs
{
    public class CreateChatModel
    {
        public IEnumerable<ObjectId> UsersIds { get; set; }
    }
}
