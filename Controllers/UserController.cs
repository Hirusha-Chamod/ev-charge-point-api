using ev_charge_point_api.Models;
using Microsoft.AspNetCore.Mvc;

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
    public Task<List<User>> GetAll() => _userService.GetAllUsersAsync();

    [HttpGet("{id}")]
    public Task<User> Get(string id) => _userService.GetUserByIdAsync(id);

    [HttpPost]
    public Task<User> Create(User user) => _userService.CreateUserAsync(user);

    [HttpPut("{id}")]
    public Task<bool> Update(string id, User user) => _userService.UpdateUserAsync(id, user);

    [HttpDelete("{id}")]
    public Task<bool> Delete(string id) => _userService.DeleteUserAsync(id);
}
