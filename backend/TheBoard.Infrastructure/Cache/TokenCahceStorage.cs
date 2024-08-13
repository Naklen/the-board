using StackExchange.Redis;
using TheBoard.Application.Interfaces;

namespace TheBoard.Infrastructure.Cache;

public class TokenCahceStorage(IConnectionMultiplexer connection) : ITokenCacheStorage
{
    private readonly IConnectionMultiplexer _connection = connection;

    // переделать логику, когда сделают реализацию HEXPIRE

    public async Task DeleteAllUserSessions(Guid userId)
    {
        var server = _connection.GetServer(_connection.GetEndPoints(true)[0]);
        var db = _connection.GetDatabase();
        var sessionsKeys = server.KeysAsync(pattern: $"{userId}:*");

        await foreach (var key in sessionsKeys)
            await db.KeyDeleteAsync(key);
    }

    public async Task DeleteBySessionId(Guid sessionId, Guid userId)
    {
        var db = _connection.GetDatabase();
        await db.KeyDeleteAsync($"{userId}:{sessionId}");
    }

    public async Task<IEnumerable<string>> GetSessionsByUserId(Guid userId)
    {
        var server = _connection.GetServer(_connection.GetEndPoints(true)[0]);
        var db = _connection.GetDatabase();
        var sessionsKeys = server.KeysAsync(pattern: $"{userId}:*");
        var result = new List<string>();

        await foreach (var key in sessionsKeys)
            result.Add(await db.StringGetAsync(key));

        return result;
    }

    public async Task<string> GetTokenBySessionId(Guid sessionId, Guid userId)
    {
        var db = _connection.GetDatabase();
        return await db.StringGetAsync($"{userId}:{sessionId}");
    }

    public async Task SetTokenBySessionId(string token, Guid sessionId, Guid userId)
    {
        var db = _connection.GetDatabase();
        await db.StringSetAsync(
            $"{userId}:{sessionId}",
            token,
            TimeSpan.FromDays(double.Parse(GetEnvironmentVariable("JWT_REFRESH_LIFETIME_DAYS"))));
    }
}
