using MongoDB.Bson;

namespace Mozaodol.Domain.Entities.StorageEntities
{
    public class Storage
    {
        public ObjectId _id { get; set; }
        public string Extension { get; set; }
        public string Name { get; set; }
        public decimal Size { get; set; }
        public CloudProvider Provider { get; set; }
    }
}
