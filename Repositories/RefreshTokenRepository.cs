using ev_charge_point_api.Models;
using ev_charge_point_api.Services;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace ev_charge_point_api.Repositories
{
    public class RefreshTokenRepository
    {
        private readonly IMongoCollection<RefreshToken> _collection;

        public RefreshTokenRepository(MongoDBService mongoDBService)
        {
            var db = mongoDBService.GetDatabase();
            _collection = db.GetCollection<RefreshToken>("refresh_tokens");
        }

        public async Task SaveTokenAsync(RefreshToken token)
        {
            await _collection.InsertOneAsync(token);
        }

        public async Task<RefreshToken?> GetByTokenAsync(string token)
        {
            return await _collection.Find(t => t.Token == token).FirstOrDefaultAsync();
        }

        public async Task DeleteAsync(string token)
        {
            await _collection.DeleteOneAsync(t => t.Token == token);
        }

        public async Task DeleteAllForUserAsync(string userId)
        {
            await _collection.DeleteManyAsync(t => t.UserId == userId);
        }
    }
}
