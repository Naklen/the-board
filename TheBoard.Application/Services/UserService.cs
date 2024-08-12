using TheBoard.Application.Auth;
using TheBoard.Application.Contracts;
using TheBoard.Application.Interfaces;
using TheBoard.Application.Stores;
using TheBoard.Core.Models;

namespace TheBoard.Application.Services;

public class UserService(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    TokenService tokenService,
    ITokenCacheStorage tokenCacheStorage)
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;
    private readonly TokenService _tokenService = tokenService;
    private readonly ITokenCacheStorage _tokenCacheStorage = tokenCacheStorage;

    public async Task<Guid> Registrate(string userName, string email, string password)
    {
        var passwordHash = _passwordHasher.Hash(password);
        var user = new User()
        {
            Id = Guid.NewGuid(),
            Username = userName,
            Email = email,
            PasswordHash = passwordHash
        };
        await _userRepository.Add(user);

        return user.Id;
    }

    public async Task<TokenPair> Login(string email, string password)
    {
        var user = await _userRepository.GetByEmail(email) ?? throw new ArgumentException("No user with this email");

        if (!_passwordHasher.Verify(password, user.PasswordHash))
            throw new ArgumentException("Wrong password");

        var newPair = _tokenService.GenerateNewTokens(user, out Guid newSessionId);

        var sessions = await _tokenCacheStorage.GetSessionsByUserId(user.Id);
        if (sessions.Count() >= int.Parse(GetEnvironmentVariable("MAX_USER_SESSIONS_COUNT")))
            _tokenCacheStorage.DeleteAllUserSessions(user.Id);

        _tokenCacheStorage.SetTokenBySessionId(newPair.RefreshToken, newSessionId, user.Id);

        return newPair;
    }

    public async Task<TokenPair> Refresh(string refreshToken)
    {
        var newPair = await _tokenService.RefreshTokens(refreshToken);

        var refreshPayload = _tokenService.GetTokenPayload(newPair.RefreshToken);
        var sessionId = Guid.Parse(refreshPayload.First(f => f.Key == "SessionId").Value);
        var userId = Guid.Parse(refreshPayload.First(f => f.Key == "UserId").Value);

        var currentRefreshToken = await _tokenCacheStorage.GetTokenBySessionId(sessionId, userId);
        if (currentRefreshToken is null)
            throw new ArgumentException("Session is not exist");
        if (currentRefreshToken != refreshToken)
        {
            _tokenCacheStorage.DeleteBySessionId(sessionId, userId);
            throw new ArgumentException("Invalid token");
        }        
        
        _tokenCacheStorage.SetTokenBySessionId(
            newPair.RefreshToken,
            sessionId,
            userId);

        return newPair;
    }

    public async Task Logout(string accessToken)
    {
        var payload = _tokenService.GetTokenPayload(accessToken);
        var sessionId = Guid.Parse(payload.First(f => f.Key == "SessionId").Value);
        var userId = Guid.Parse(payload.First(f => f.Key == "UserId").Value);

        await _tokenCacheStorage.DeleteBySessionId(sessionId, userId);
    }

    public async Task<User> GetById(Guid id)
    {
        return await _userRepository.GetById(id);
    }
}
