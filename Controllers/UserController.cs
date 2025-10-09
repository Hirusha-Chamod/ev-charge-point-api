// --------------------------------------------------------------------------------------------------------------------
// <project>EV Charging Station Management</project>
// <file>AuthController.cs</file>
// <author>GOMIS R J S (IT22349606)</author>
// <module>SE4040 - Enterprise Application Development</module>
// <date>2025-10-10</date>
// <summary>
//   Exposes API endpoints for managing all users. BackOffice users can only access all user managing endpoints
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using ev_charge_point_api.Dtos;
using ev_charge_point_api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = "BackOffice")]
[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly UserService _userService;

    public UserController(UserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public Task<List<UserResponseDto>> GetAll() => _userService.GetAllUsersAsync();

    [HttpGet("{id}")]
    public Task<User> Get(string id) => _userService.GetUserByIdAsync(id);

    [HttpPost]
    public Task<User> Create(User user) => _userService.CreateUserAsync(user);

    [HttpPut("{id}")]
    public Task<bool> Update(string id, UserResponseDto user) => _userService.UpdateUserAsync(id, user);

    [HttpDelete("{id}")]
    public Task<bool> Delete(string id) => _userService.DeleteUserAsync(id);
}
