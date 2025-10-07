using ev_charge_point_api.Models;
using ev_charge_point_api.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

public class UserService
{
    private readonly UserRepository _userRepository;

    public UserService(UserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public Task<User> CreateUserAsync(User user)
    {
        // You could add validations, hashing passwords, etc. here
        return _userRepository.CreateAsync(user);
    }

    public Task<List<User>> GetAllUsersAsync() => _userRepository.GetAllAsync();

    public Task<User> GetUserByIdAsync(string id) => _userRepository.GetByIdAsync(id);

    public Task<bool> UpdateUserAsync(string id, User user) => _userRepository.UpdateAsync(id, user);

    public Task<bool> DeleteUserAsync(string id) => _userRepository.DeleteAsync(id);
}
