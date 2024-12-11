using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Mozaodol.Infrastructure.Contexts.MongoDBContexts
{
    public class MongoDBContext
    {
        public IMongoDatabase Database { get; set; }
        public MongoDBContext(IMongoClient mongoClient, IOptions<MongoDBSettings> mongoDBSettings)
        {
            Database = mongoClient.GetDatabase(mongoDBSettings.Value.DatabaseName);
            ConnectionString = mongoDBSettings.Value.ConnectionString;
            DatabaseName = mongoDBSettings.Value.DatabaseName;
        }

        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }

    }
}
