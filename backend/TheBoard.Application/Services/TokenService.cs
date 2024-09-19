using TheBoard.Application.Auth;
using TheBoard.Application.Contracts;
using TheBoard.Application.Errors;
using TheBoard.Application.Stores;
using TheBoard.Core.Models;

namespace TheBoard.Application.Services;

public class TokenService(IJwtProvider jwtProvider, IUserRepository userRepository)
{
    private readonly IJwtProvider _jwtProvider = jwtProvider;
    private readonly IUserRepository _userRepository = userRepository;

    public string GenerateAccessToken(User user, Guid sessionId)
    {
        var accessSecret = GetEnvironmentVariable("JWT_ACCESS_SECRET");
        var accessExpires = DateTime.UtcNow.AddMinutes(double.Parse(GetEnvironmentVariable("JWT_ACCESS_LIFETIME_MINUTES")));
        var accessToken = _jwtProvider.GenerateToken(
            [
                new ("UserId", user.Id.ToString()),
                new ("SessionId", sessionId.ToString())
            ],
            accessSecret,
            accessExpires);
        return accessToken;
    }

    public string GenerateRefreshTokens(User user, Guid sessionId, Guid tokenId)
    {
        var refreshSecret = GetEnvironmentVariable("JWT_REFRESH_SECRET");
        var refreshExpires = DateTime.UtcNow.AddDays(double.Parse(GetEnvironmentVariable("JWT_REFRESH_LIFETIME_DAYS")));
        var refreshToken = _jwtProvider.GenerateToken(
            [
                new ("UserId", user.Id.ToString()),
                new ("SessionId", sessionId.ToString()),
                new ("TokenId", tokenId.ToString())
            ],
            refreshSecret,
            refreshExpires);
        return refreshToken;
    }

    public TokenPair GenerateNewTokens(User user, out Guid newSessionId)
    {
        newSessionId = Guid.NewGuid();
        var accessToken = GenerateAccessToken(user, newSessionId);
        var newRefreshId = Guid.NewGuid();
        var refreshToken = GenerateRefreshTokens(user, newSessionId, newRefreshId);

        return new TokenPair(accessToken, refreshToken);
    }

    public async Task<Result<TokenPair>> RefreshTokens(string refreshTokenString)
    {
        if (!_jwtProvider.VerifyToken(refreshTokenString, GetEnvironmentVariable("JWT_REFRESH_SECRET"), out var verifiedPayload))
            return AuthError.InvalidRefreshToken();

        var userId = Guid.Parse(verifiedPayload.FirstOrDefault(f => f.Key == "UserId").Value);
        var sessionId = Guid.Parse(verifiedPayload.FirstOrDefault(f => f.Key == "SessionId").Value);
        var user = await _userRepository.GetById(userId);
        if (user is null)
            return UserError.IsNotExist();

        var newAccessToken = GenerateAccessToken(user, sessionId);
        var newRefreshToken = GenerateRefreshTokens(user, sessionId, Guid.NewGuid());

        return new TokenPair(newAccessToken, newRefreshToken);
    }

    public IEnumerable<TokenPayloadField> GetTokenPayload(string token)
    {
        var payload = _jwtProvider.GetTokenPayloadJson(token);
        return payload.AsObject().Select(f => new TokenPayloadField(f.Key, f.Value.ToString()));
    }
}
