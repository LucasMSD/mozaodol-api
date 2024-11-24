using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace ProjetoTelegram.Domain.Repositories
{
    public class DbContext
    {
        public IMongoDatabase Database { get; set; }
        public DbContext(IOptions<MongoDBSettings> mongoDBSettings, IMongoClient mongoClient)
        {
            Database = mongoClient.GetDatabase(mongoDBSettings.Value.DatabaseName);
            ConnectionString = mongoDBSettings.Value.ConnectionString;
            DatabaseName = mongoDBSettings.Value.DatabaseName;
        }

        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }

    }
}
