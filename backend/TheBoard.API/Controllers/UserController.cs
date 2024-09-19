using FluentResults;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheBoard.API.Contracts.Errors;
using TheBoard.API.Contracts.User;
using TheBoard.API.Features;
using TheBoard.Application.Contracts;
using TheBoard.Application.Errors;
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
        var registrateResult = await _userService.Registrate(user.Username, user.Email, user.Password);

        if (registrateResult.IsFailed)
            return BadRequest(new ResponseErrors(registrateResult.Errors));

        return CreatedAtAction(nameof(GetUser), new { registrateResult.Value }, new GetUserResponse(registrateResult.Value, user.Email, user.Username));
    }

    [HttpPost("login")]
    [AllowAnonymousOnly]
    public async Task<ActionResult<AccessTokenDataResponse>> Login([FromBody] LoginUserRequest credintials)
    {
        var loginResult = await _userService.Login(credintials.Email, credintials.Password);

        if (loginResult.IsFailed)
            return Unauthorized(new ResponseErrors(loginResult.Errors));

        var accessData = SetAccessAndRefreshTokensToCookie(loginResult.Value);

        return new AccessTokenDataResponse(accessData);
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<GetUserResponse>> GetUser(Guid id)
    {
        var (isSuccess, isFailed, user, errors) = await _userService.GetById(id);
        if (isFailed)
            return NotFound(new ResponseErrors(errors));
        return new GetUserResponse(id, user.Email, user.Username);
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<GetUserResponse>> GetUserSelf()
    {
        HttpContext.Request.Headers.TryGetValue(GetEnvironmentVariable("JWT_ACCESS_HEADER_NAME"), out var token);
        var userId = Guid.Parse(_tokenService.GetTokenPayload(token).First(f => f.Key == "UserId").Value);
        return await GetUser(userId);
    }

    [HttpGet("refresh")]
    public async Task<ActionResult<AccessTokenDataResponse>> Refresh()
    {
        if (!HttpContext.Request.Cookies.TryGetValue(GetEnvironmentVariable("JWT_REFRESH_COOKIE_NAME"), out var token))
            return Unauthorized();

        var refreshResult = await _userService.Refresh(token);
        if (refreshResult.IsFailed)
        {
            if (refreshResult.HasError(e => e.HasMetadata("code", m => (ErrorType)m == ErrorType.NotFound)))
                KillAccessAndrefreshCookies();
            return Unauthorized();
        }

        var tokenPair = refreshResult.Value;
        var accessData = SetAccessAndRefreshTokensToCookie(tokenPair);
        return new AccessTokenDataResponse(accessData);
    }

    [HttpGet("logout")]
    [Authorize]
    public async Task<ActionResult<string>> Logout()
    {
        if (HttpContext.Request.Headers.TryGetValue(GetEnvironmentVariable("JWT_ACCESS_HEADER_NAME"), out var token))
        {
            await _userService.Logout(token);
            KillAccessAndrefreshCookies();
        }
        return Ok();
    }

    private string SetAccessAndRefreshTokensToCookie(TokenPair tokenPair)
    {
        var accessTokenSplitRes = tokenPair.AccessToken.Split('.');
        HttpContext.Response.Cookies.Append(
            GetEnvironmentVariable("JWT_ACCESS_COOKIE_NAME"),
            accessTokenSplitRes[2],
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
        return accessTokenSplitRes[0] + '.' + accessTokenSplitRes[1];
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
