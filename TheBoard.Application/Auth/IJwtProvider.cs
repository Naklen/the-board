using System.Text.Json.Nodes;
using TheBoard.Application.Contracts;

namespace TheBoard.Application.Auth;

public interface IJwtProvider
{
    string GenerateToken(IEnumerable<TokenPayloadField> payload, string secretKey, DateTime expiresIn);
    bool VerifyToken(string tokenString, string secretKey, out IEnumerable<TokenPayloadField> verifiedPayload);
    JsonNode GetTokenPayloadJson(string tokenString);
}
