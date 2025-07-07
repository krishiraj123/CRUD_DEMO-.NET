using MongoDB.Driver;

namespace myapi.Services
{
    public class MongoServices
    {
        private readonly IMongoDatabase mongoDatabase;
        private readonly IConfiguration configuration;

        public MongoServices(IConfiguration configuration)
        {
            this.configuration = configuration;
            var client = new MongoClient(configuration.GetConnectionString("MongoDBConnectionString"));
            mongoDatabase = client.GetDatabase(configuration.GetConnectionString("DatabaseName"));
        }

        public IMongoCollection<T> GetCollection<T>(string collectionName)
        {
            return mongoDatabase.GetCollection<T>(collectionName);
        }
    }
}
