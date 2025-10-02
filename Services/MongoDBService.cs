using MongoDB.Driver;

namespace ev_charge_point_api.Services
{
    public class MongoDBService
    {
        private readonly IMongoDatabase _database;
        private readonly ILogger<MongoDBService> _logger;

        public MongoDBService(IConfiguration configuration, ILogger<MongoDBService> logger)
        {
            _logger = logger;

            var connectionString = configuration.GetConnectionString("MongoDB");
            var databaseName = configuration.GetValue<string>("DatabaseName") ?? "EVChargingDB";

            // Log the connection details
            _logger.LogInformation("🔌 Attempting MongoDB connection...");


            if (string.IsNullOrEmpty(connectionString))
            {
                _logger.LogError("MongoDB connection string is null or empty");
                throw new ArgumentException("MongoDB connection string is null or empty");
            }

            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(databaseName);

            _logger.LogInformation("Successfully connected to MongoDB: {DatabaseName}", databaseName);


        }


        public IMongoDatabase GetDatabase() => _database;

        public IMongoCollection<T> GetCollection<T>(string collectionName)
        {
            return _database.GetCollection<T>(collectionName);
        }
    }
}