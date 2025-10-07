using ev_charge_point_api.Models;
using ev_charge_point_api.Services;
using MongoDB.Driver;

namespace ev_charge_point_api.Repositories
{
    public class UserRepository
    {
        private readonly IMongoCollection<User> _users;

        public UserRepository(MongoDBService database)
        {
            _users = database.GetCollection<User>("Users");
        }

        // Create
        public async Task<User> CreateAsync(User user)
        {
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            await _users.InsertOneAsync(user);
            return user;
        }

        // Read all
        public async Task<List<User>> GetAllAsync()
        {
            return await _users.Find(u => true).ToListAsync();
        }

        // Read by Id
        public async Task<User> GetByIdAsync(string id)
        {
            return await _users.Find(u => u.Id == id).FirstOrDefaultAsync();
        }

        // Read by Email
        public async Task<User> GetByEmailAsync(string email)
        {
            return await _users.Find(u => u.Email == email).FirstOrDefaultAsync();
        }

        // Update
        public async Task<bool> UpdateAsync(string id, User updatedUser)
        {
            var result = await _users.ReplaceOneAsync(u => u.Id == id, updatedUser);
            return result.ModifiedCount > 0;
        }

        // Delete
        public async Task<bool> DeleteAsync(string id)
        {
            var result = await _users.DeleteOneAsync(u => u.Id == id);
            return result.DeletedCount > 0;
        }
    }
}
