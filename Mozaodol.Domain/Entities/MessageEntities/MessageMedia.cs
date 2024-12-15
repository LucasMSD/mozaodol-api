using MongoDB.Bson;

namespace Mozaodol.Domain.Entities.MessageEntities
{
    public class MessageMedia
    {
        public MessageMediaType Type { get; set; }
        public ObjectId StorageId { get; set; }
    }
}
