using MongoDB.Bson;

namespace Mozaodol.Application.DTOs.UserDTOs
{
    public class UserDto
    {
        public ObjectId _id { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string PushToken { get; set; }
    }
}
