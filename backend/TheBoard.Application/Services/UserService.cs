using TheBoard.Application.Auth;
using TheBoard.Application.Contracts;
using TheBoard.Application.Errors;
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

    public async Task<Result<Guid>> Registrate(string username, string email, string password)
    {
        var emailIsExistResult = Result.FailIf(
            await _userRepository.GetByEmail(email) != null,
            UserError.RegistrationWithExistedEmail(email));

        var usernameIsExistResult = Result.FailIf(
            await _userRepository.GetByUsername(username) != null,
            UserError.RegistrationWithExistedUsername(username));

        if (emailIsExistResult.IsFailed || usernameIsExistResult.IsFailed)
            return Result.Merge(emailIsExistResult, usernameIsExistResult);
        var passwordHash = _passwordHasher.Hash(password);
        var user = new User()
        {
            Id = Guid.NewGuid(),
            Username = username,
            Email = email,
            PasswordHash = passwordHash
        };
        await _userRepository.Add(user);

        return user.Id;
    }

    public async Task<Result<TokenPair>> Login(string email, string password)
    {
        var user = await _userRepository.GetByEmail(email);

        if (user is null)
            return Result.Fail(AuthError.LoginToNotExistedUser("email", email));

        if (!_passwordHasher.Verify(password, user.PasswordHash))
            return Result.Fail(AuthError.WrongPassword());

        var newPair = _tokenService.GenerateNewTokens(user, out Guid newSessionId);

        var sessions = await _tokenCacheStorage.GetSessionsByUserId(user.Id);
        if (sessions.Count() >= int.Parse(GetEnvironmentVariable("MAX_USER_SESSIONS_COUNT")))
            await _tokenCacheStorage.DeleteAllUserSessions(user.Id);

        await _tokenCacheStorage.SetTokenBySessionId(newPair.RefreshToken, newSessionId, user.Id);

        return newPair;
    }

    public async Task<Result<TokenPair>> Refresh(string refreshToken)
    {
        var refreshResult = await _tokenService.RefreshTokens(refreshToken);
        if (refreshResult.IsFailed)
            return refreshResult;
        var newPair = refreshResult.Value;

        var refreshPayload = _tokenService.GetTokenPayload(newPair.RefreshToken);
        var sessionId = Guid.Parse(refreshPayload.First(f => f.Key == "SessionId").Value);
        var userId = Guid.Parse(refreshPayload.First(f => f.Key == "UserId").Value);

        var currentRefreshToken = await _tokenCacheStorage.GetTokenBySessionId(sessionId, userId);
        if (currentRefreshToken is null)
            return AuthError.SessionIsNotExist();
        if (currentRefreshToken != refreshToken)
        {
            await _tokenCacheStorage.DeleteBySessionId(sessionId, userId);
            return AuthError.InvalidRefreshToken();
        }

        await _tokenCacheStorage.SetTokenBySessionId(
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

    public async Task<Result<User>> GetById(Guid id)
    {
        var user = await _userRepository.GetById(id);
        var result = Result.FailIf(user is null, UserError.IsNotExist);
        return result;
    }
}
