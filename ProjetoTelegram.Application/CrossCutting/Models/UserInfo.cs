using MongoDB.Bson;

namespace ProjetoTelegram.Application.CrossCutting.Models
{
    public class UserInfo
    {
        public ObjectId Id { get; set; }
        public string Username { get; set; }
        public string Connection { get; set; }
    }
}
