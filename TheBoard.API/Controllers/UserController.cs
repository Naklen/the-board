using Microsoft.AspNetCore.Mvc;
using TheBoard.API.Contracts.User;
using TheBoard.Application.Services;

namespace TheBoard.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly UserService _userService;

    public UserController(UserService userService)
    {
        _userService = userService;
    }

    [HttpPost]
    public async Task<IActionResult> Registrate([FromBody] RegistrateUserRequest user)
    {
        await _userService.Registrate(user.UserName, user.Email, user.Password);

        return Created();
    }
}
