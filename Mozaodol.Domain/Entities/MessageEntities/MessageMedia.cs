using MongoDB.Bson;

namespace Mozaodol.Domain.Entities.MessageEntities
{
    public class MessageMedia
    {
        public MediaType Type { get; set; }
        public ObjectId StorageId { get; set; }
    }
}
