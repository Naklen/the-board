namespace TheBoard.Application.Interfaces;

public interface ITokenCacheStorage
{
    Task<string> GetTokenBySessionId(Guid sessionId, Guid userId);
    Task SetTokenBySessionId(string token, Guid sessionId, Guid userId);
    Task<IEnumerable<string>> GetSessionsByUserId(Guid userId);
    Task DeleteBySessionId(Guid sessionId, Guid userId);
    Task DeleteAllUserSessions(Guid userId);
}