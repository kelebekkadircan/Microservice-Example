using MongoDB.Driver;

namespace STOCK.API.Services
{
    public class MongoDBService
    {
        readonly IMongoDatabase _mongoDatabase;

        public MongoDBService(IConfiguration configuration)
        {
            MongoClient client = new(configuration.GetConnectionString("DefaultConnection"));
            _mongoDatabase = client.GetDatabase("StockApiDB");
        }

        public IMongoCollection<T> GetCollection<T>()
        {
            return _mongoDatabase.GetCollection<T>(typeof(T).Name.ToLowerInvariant());
        }

    }
}
