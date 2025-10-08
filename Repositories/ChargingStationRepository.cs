// --------------------------------------------------------------------------------------------------------------------
// <project>EV Charging Station Management</project>
// <file>ChargingStationRepository.cs</file>
// <author>Jayarathne H.C.D (IT22311290)</author>
// <module>SE4040 - Enterprise Application Development</module>
// <date>2025-10-06</date>
// <summary>
//   Implements the data access logic for ChargingStation entities using MongoDB.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using ev_charge_point_api.Models;
using ev_charge_point_api.Services;
using MongoDB.Driver;
using MongoDB.Driver.GeoJsonObjectModel;

namespace ev_charge_point_api.Repositories
{
    public class ChargingStationRepository : IChargingStationRepository
    {
        private readonly IMongoCollection<ChargingStation> _stationsCollection;

        // Initializes a new instance of the ChargingStationRepository class.
        public ChargingStationRepository(MongoDBService mongoDBService)
        {
            _stationsCollection = mongoDBService.GetCollection<ChargingStation>("chargingStations");

            // Ensures the 2dsphere index exists for efficient geospatial queries.
            var indexKeysDefinition = Builders<ChargingStation>.IndexKeys.Geo2DSphere(station => station.Location);
            _stationsCollection.Indexes.CreateOne(new CreateIndexModel<ChargingStation>(indexKeysDefinition));
        }

        // Adds a new charging station document to the collection.
        public async Task CreateAsync(ChargingStation station) =>
            await _stationsCollection.InsertOneAsync(station);

        // Retrieves all charging station documents from the collection.
        public async Task<IEnumerable<ChargingStation>> GetAllAsync() =>
            await _stationsCollection.Find(_ => true).ToListAsync();

        // Retrieves a single charging station document by its unique ID.
        public async Task<ChargingStation?> GetByIdAsync(string id) =>
            await _stationsCollection.Find(s => s.Id == id).FirstOrDefaultAsync();

        // Finds all charging stations within a specified distance of a geographic point.
        public async Task<IEnumerable<ChargingStation>> GetNearbyAsync(double longitude, double latitude, int maxDistanceInMeters = 5000)
        {
            var point = GeoJson.Point(GeoJson.Position(longitude, latitude));
            var filter = Builders<ChargingStation>.Filter.Near(s => s.Location, point, maxDistanceInMeters);

            return await _stationsCollection.Find(filter).ToListAsync();
        }

        // Replaces an existing charging station document with an updated version.
        public async Task<bool> UpdateAsync(ChargingStation station)
        {
            var filter = Builders<ChargingStation>.Filter.Eq(s => s.Id, station.Id);

            var update = Builders<ChargingStation>.Update
                .Set(s => s.Name, station.Name)
                .Set(s => s.Type, station.Type)
                .Set(s => s.Location, station.Location);

            var result = await _stationsCollection.UpdateOneAsync(filter, update);

            return result.IsAcknowledged && result.ModifiedCount > 0;
        }

        // Sets the IsActive flag to false for a specific charging station document.
        public async Task<bool> DeactivateAsync(string id)
        {
            var filter = Builders<ChargingStation>.Filter.Eq(s => s.Id, id);
            var update = Builders<ChargingStation>.Update.Set(s => s.IsActive, false);
            var result = await _stationsCollection.UpdateOneAsync(filter, update);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }
    }
}