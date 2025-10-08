// --------------------------------------------------------------------------------------------------------------------
// <project>EV Charging Station Management</project>
// <file>MongoDBService.cs</file>
// <author>Jayarathne H.C.D (IT22311290)</author>
// <module>SE4040 - Enterprise Application Development</module>
// <date>2025-10-02</date>
// <summary>
//   Handles the connection to the MongoDB database and provides access to its collections.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using MongoDB.Driver;

namespace ev_charge_point_api.Services
{
    public class MongoDBService
    {
        private readonly IMongoDatabase _database;
        private readonly ILogger<MongoDBService> _logger;

        // Initializes a new instance of the MongoDBService class and establishes the database connection.
        public MongoDBService(IConfiguration configuration, ILogger<MongoDBService> logger)
        {
            _logger = logger;

            var connectionString = configuration.GetConnectionString("MongoDB");
            var databaseName = configuration.GetValue<string>("DatabaseName") ?? "EVChargingDB";

            _logger.LogInformation("Attempting MongoDB connection...");

            if (string.IsNullOrEmpty(connectionString))
            {
                _logger.LogError("MongoDB connection string is null or empty");
                throw new ArgumentException("MongoDB connection string is null or empty");
            }

            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(databaseName);

            _logger.LogInformation("Successfully connected to MongoDB: {DatabaseName}", databaseName);
        }

        // Gets the connected MongoDB database instance.
        public IMongoDatabase GetDatabase() => _database;

        // Gets a specific collection from the database by name.
        public IMongoCollection<T> GetCollection<T>(string collectionName)
        {
            return _database.GetCollection<T>(collectionName);
        }
    }
}