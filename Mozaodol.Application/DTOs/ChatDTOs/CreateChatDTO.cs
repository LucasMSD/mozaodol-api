using MongoDB.Bson;

namespace Mozaodol.Application.DTOs.ChatDTOs
{
    public class CreateChatModel
    {
        public IEnumerable<ObjectId> UsersIds { get; set; }
    }
}
