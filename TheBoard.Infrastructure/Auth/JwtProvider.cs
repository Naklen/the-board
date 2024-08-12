using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Nodes;
using TheBoard.Application.Auth;
using TheBoard.Application.Contracts;

namespace TheBoard.Infrastructure.Auth;

public class JwtProvider : IJwtProvider
{
    public string GenerateToken(IEnumerable<TokenPayloadField> payload, string secretKey, DateTime expiresIn)
    {
        var claims = payload.Select(f => new Claim(f.Key, f.Value)).ToArray();

        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            claims: claims,
            signingCredentials: signingCredentials,
            expires: expiresIn);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public bool VerifyToken(string tokenString, string secretKey, out IEnumerable<TokenPayloadField> verifiedPayload)
    {
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(secretKey))
        };
        try
        {
            verifiedPayload = new JwtSecurityTokenHandler()
                .ValidateToken(tokenString, validationParameters, out SecurityToken token)
                .Claims
                .Select(c => new TokenPayloadField(c.Type, c.Value))
                .ToArray();
            return true;
        }
        catch (SecurityTokenValidationException)
        {
            verifiedPayload = null;
            return false;
        }
    }

    public JsonNode GetTokenPayloadJson(string tokenString)
    {
        var json = Base64UrlEncoder.Decode(tokenString.Split('.')[1]);
        return JsonNode.Parse(json);
    }
}
