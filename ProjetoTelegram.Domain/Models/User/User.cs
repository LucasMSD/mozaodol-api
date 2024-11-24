using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ProjetoTelegram.Domain.Models.User
{
    [BsonIgnoreExtraElements]
    public class User
    {
        public ObjectId _id { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string PushToken { get; set; }
        public List<ObjectId> Contacts { get; set; }
        public List<ObjectId> ChatsIds { get; set; }
    }
}
