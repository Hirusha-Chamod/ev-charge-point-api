// --------------------------------------------------------------------------------------------------------------------
// <project>EV Charging Station Management</project>
// <file>AuthController.cs</file>
// <author>GOMIS R J S (IT22349606)</author>
// <module>SE4040 - Enterprise Application Development</module>
// <date>2025-10-10</date>
// <summary>
//   Implements the data access logic for Station operator and Back officer users
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using ev_charge_point_api.Dtos;
using ev_charge_point_api.Models;
using ev_charge_point_api.Services;
using MongoDB.Bson;
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
        public async Task<List<UserResponseDto>> GetAllAsync()
        {
            return await _users.Find(u => true)
                .Project(u => new UserResponseDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email,
                    Role = u.Role,
                })
                .ToListAsync();
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
        public async Task<bool> UpdateAsync(string id, UserResponseDto updatedUser)
        {
            var update = Builders<User>.Update
                .Set(u => u.Name, updatedUser.Name)
                .Set(u => u.Email, updatedUser.Email)
                .Set(u => u.Role, updatedUser.Role);

            var result = await _users.UpdateOneAsync(u =>  u.Id == id, update);
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
