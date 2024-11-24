using MongoDB.Bson;

namespace ProjetoTelegram.Domain.Models.User
{
    public class UserDto
    {
        public ObjectId _id { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string PushToken { get; set; }
    }
}
