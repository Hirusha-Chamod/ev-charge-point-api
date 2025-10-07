using ev_charge_point_api.Models;
using ev_charge_point_api.Services;
using MongoDB.Driver;
using MongoDB.Driver.GeoJsonObjectModel;

namespace ev_charge_point_api.Repositories
{
    public class ChargingStationRepository : IChargingStationRepository
    {
        private readonly IMongoCollection<ChargingStation> _stationsCollection;

        public ChargingStationRepository(MongoDBService mongoDBService)
        {
            
            _stationsCollection = mongoDBService.GetCollection<ChargingStation>("chargingStations");

            
            var indexKeysDefinition = Builders<ChargingStation>.IndexKeys.Geo2DSphere(station => station.Location);
            _stationsCollection.Indexes.CreateOne(new CreateIndexModel<ChargingStation>(indexKeysDefinition));
        }

        public async Task CreateAsync(ChargingStation station) =>
            await _stationsCollection.InsertOneAsync(station);

        public async Task<IEnumerable<ChargingStation>> GetAllAsync() =>
            await _stationsCollection.Find(_ => true).ToListAsync();

        public async Task<ChargingStation?> GetByIdAsync(string id) =>
            await _stationsCollection.Find(s => s.Id == id).FirstOrDefaultAsync();

        // The implementation for the mobile app's "nearby" feature
        public async Task<IEnumerable<ChargingStation>> GetNearbyAsync(double longitude, double latitude, int maxDistanceInMeters = 5000)
        {
            var point = GeoJson.Point(GeoJson.Position(longitude, latitude));
            var filter = Builders<ChargingStation>.Filter.Near(s => s.Location, point, maxDistanceInMeters);
            
            return await _stationsCollection.Find(filter).ToListAsync();
        }

        public async Task<bool> UpdateAsync(ChargingStation station)
        {
            var result = await _stationsCollection.ReplaceOneAsync(s => s.Id == station.Id, station);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }

        public async Task<bool> DeactivateAsync(string id)
        {
            var filter = Builders<ChargingStation>.Filter.Eq(s => s.Id, id);
            var update = Builders<ChargingStation>.Update.Set(s => s.IsActive, false);
            var result = await _stationsCollection.UpdateOneAsync(filter, update);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }
    }
}