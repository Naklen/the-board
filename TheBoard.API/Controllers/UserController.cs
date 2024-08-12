using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheBoard.API.Contracts.User;
using TheBoard.API.Features;
using TheBoard.Application.Contracts;
using TheBoard.Application.Services;

namespace TheBoard.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController(UserService userService, TokenService tokenService) : ControllerBase
{
    private readonly UserService _userService = userService;
    private readonly TokenService _tokenService = tokenService;

    [HttpPost("registrate")]
    [AllowAnonymousOnly]
    public async Task<ActionResult<GetUserResponse>> Registrate([FromBody] RegistrateUserRequest user)
    {
        var id = await _userService.Registrate(user.Username, user.Email, user.Password);

        return CreatedAtAction(nameof(GetUser), new { id }, new GetUserResponse(id, user.Email, user.Username));
    }

    [HttpPost("login")]
    [AllowAnonymousOnly]
    public async Task<IActionResult> Login([FromBody] LoginUserRequest credintials)
    {
        var tokenPair = await _userService.Login(credintials.Email, credintials.Password);

        SetAccessAndRefreshTokensToCookie(tokenPair);

        return Ok();
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<GetUserResponse>> GetUser(Guid id)
    {
        var user = await _userService.GetById(id);
        return new GetUserResponse(id, user.Email, user.Username);
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<GetUserResponse>> GetUserSelf()
    {
        HttpContext.Request.Cookies.TryGetValue(GetEnvironmentVariable("JWT_ACCESS_COOKIE_NAME"), out var token);
        var userId = Guid.Parse(_tokenService.GetTokenPayload(token).First(f => f.Key == "UserId").Value);
        return await GetUser(userId);
    }

    [HttpGet("refresh")]
    public async Task<ActionResult<string>> Refresh()
    {
        if (!HttpContext.Request.Cookies.TryGetValue(GetEnvironmentVariable("JWT_REFRESH_COOKIE_NAME"), out var token))
            return Unauthorized();
        try
        {
            var tokenPair = await _userService.Refresh(token);
            SetAccessAndRefreshTokensToCookie(tokenPair);
        }
        catch (ArgumentException ex)
        {
            return Unauthorized();
        }

        return Ok();
    }

    [HttpGet("logout")]
    [Authorize]
    public async Task<ActionResult<string>> Logout()
    {
        if (HttpContext.Request.Cookies.TryGetValue(GetEnvironmentVariable("JWT_ACCESS_COOKIE_NAME"), out var token))
        {
            await _userService.Logout(token);
            KillAccessAndrefreshCookies();
        }
        return Ok();
    }

    private void SetAccessAndRefreshTokensToCookie(TokenPair tokenPair)
    {
        HttpContext.Response.Cookies.Append(
            GetEnvironmentVariable("JWT_ACCESS_COOKIE_NAME"),
            tokenPair.AccessToken,
            new CookieOptions()
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddMinutes(double.Parse(GetEnvironmentVariable("JWT_ACCESS_LIFETIME_MINUTES")))
            });

        HttpContext.Response.Cookies.Append(
            GetEnvironmentVariable("JWT_REFRESH_COOKIE_NAME"),
            tokenPair.RefreshToken,
            new CookieOptions()
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Path = "/api/user/refresh",
                Expires = DateTime.UtcNow.AddDays(double.Parse(GetEnvironmentVariable("JWT_REFRESH_LIFETIME_DAYS")))
            });
    }

    private void KillAccessAndrefreshCookies()
    {
        HttpContext.Response.Cookies.Append(
            GetEnvironmentVariable("JWT_ACCESS_COOKIE_NAME"),
            string.Empty,
            new CookieOptions()
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.MinValue
            });

        HttpContext.Response.Cookies.Append(
            GetEnvironmentVariable("JWT_REFRESH_COOKIE_NAME"),
            string.Empty,
            new CookieOptions()
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Path = "/api/user/refresh",
                Expires = DateTimeOffset.MinValue
            });
    }
}
