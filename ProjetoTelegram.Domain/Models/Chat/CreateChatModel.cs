using MongoDB.Bson;

namespace ProjetoTelegram.Domain.Models.Chat
{
    public class CreateChatModel
    {
        public IEnumerable<ObjectId> UsersIds { get; set; }
    }
}
