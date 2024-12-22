using MongoDB.Bson;
using Mozaodol.Domain.Entities.MessageEntities;

namespace Mozaodol.Domain.Entities.StorageEntities
{
    public class Storage
    {
        public ObjectId _id { get; set; }
        public MediaType Type { get; set; }
        public ObjectId UserId { get; set; }
        public string Extension { get; set; }
        public decimal Size { get; set; }
        public CloudProvider Provider { get; set; }
    }
}
