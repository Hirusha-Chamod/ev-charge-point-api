using ev_charge_point_api.Models;
using ev_charge_point_api.Services;
using MongoDB.Driver;

namespace ev_charge_point_api.Repositories
{
    public class EvUserRepository
    {
        private readonly IMongoCollection<EvUser> _users;

        public EvUserRepository(MongoDBService database)
        {
            _users = database.GetCollection<EvUser>("EvUsers");
        }

        public async Task<EvUser> CreateAsync(EvUser user)
        {
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            await _users.InsertOneAsync(user);
            return user;
        }

        public async Task<List<EvUser>> GetAllAsync() => await _users.Find(u => true).ToListAsync();

        public async Task<EvUser> GetByNicAsync(string nic) => await _users.Find(u => u.Nic == nic).FirstOrDefaultAsync();

        public async Task<EvUser> GetByEmailAsync(string email) => await _users.Find(u => u.Email == email).FirstOrDefaultAsync();

        public async Task<bool> UpdateAsync(string nic, EvUser updated)
        {
            var result = await _users.ReplaceOneAsync(u => u.Nic == nic, updated);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> DeactivateAsync(string nic, string deactivatedBy, string? reason = null)
        {
            var update = Builders<EvUser>.Update
                .Set(u => u.IsActive, false)
                .Set(u => u.DeactivatedBy, deactivatedBy)
                .Set(u => u.DeactivatedAt, DateTime.UtcNow)
                .Set(u => u.DeactivationReason, reason);

            var result = await _users.UpdateOneAsync(u => u.Nic == nic, update);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> ReactivateAsync(string nic)
        {
            var update = Builders<EvUser>.Update
                .Set(u => u.IsActive, true)
                .Unset(u => u.DeactivatedBy)
                .Unset(u => u.DeactivatedAt);

            var result = await _users.UpdateOneAsync(u => u.Nic == nic, update);
            return result.ModifiedCount > 0;
        }
    }
}
