using MongoDB.Driver;

namespace ServiceB.Services
{
    public class MongoDBService
    {
        IMongoDatabase _mongoDatabase;

        public MongoDBService(IConfiguration configuration)
        {
            MongoClient client = new( configuration.GetConnectionString("MongoDB"));
                _mongoDatabase = client.GetDatabase("ServiceBDB");
        }

        public IMongoCollection<T> GetCollection<T>() => _mongoDatabase.GetCollection<T>(typeof(T).Name.ToLowerInvariant()); 
    }
}
