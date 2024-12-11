using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Mozaodol.Domain.Entities.UserEntities;

namespace Mozaodol.Domain.Entities.ChatEntities
{
    [BsonIgnoreExtraElements]
    public class Chat
    {
        public ObjectId _id { get; set; }
        public IEnumerable<ObjectId> UsersIds { get; set; }


        public static string GenerateChatName(User[] users)
        {
            var name = "";

            for (var i = 0; i < users.Length; i++)
            {
                if (i != 0)
                {
                    if (i == users.Length - 1)
                        name += " e ";
                    else
                        name += ", ";
                }

                name += users[i].Username;
            }

            return name;
        }
    }
}
