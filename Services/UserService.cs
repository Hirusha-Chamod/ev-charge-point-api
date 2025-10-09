// --------------------------------------------------------------------------------------------------------------------
// <project>EV Charging Station Management</project>
// <file>AuthController.cs</file>
// <author>GOMIS R J S (IT22349606)</author>
// <module>SE4040 - Enterprise Application Development</module>
// <date>2025-10-10</date>
// <summary>
//   Implements the business logic for managing station operator and back office users, acting as the mediator
//   between the controller and the repository.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using ev_charge_point_api.Dtos;
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

    public Task<List<UserResponseDto>> GetAllUsersAsync() => _userRepository.GetAllAsync();

    public Task<User> GetUserByIdAsync(string id) => _userRepository.GetByIdAsync(id);

    public Task<bool> UpdateUserAsync(string id, UserResponseDto user) => _userRepository.UpdateAsync(id, user);

    public Task<bool> DeleteUserAsync(string id) => _userRepository.DeleteAsync(id);
}
