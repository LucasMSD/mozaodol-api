using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ProjetoTelegram.Domain.Entities.ChatEntities
{
    [BsonIgnoreExtraElements]
    public class Chat
    {
        public ObjectId _id { get; set; }
        public IEnumerable<ObjectId> UsersIds { get; set; }
    }
}
