using TheBoard.Infrastructure.Auth;

namespace TheBoard.Tests.Infrastructure;

public class JwtProviderTests
{
    //tokens encoded on https://jwt.io/#debugger-io

    [Fact]
    public void GenerateValidToken()
    {
        var jwtProvider = new JwtProvider();
        var secretKey = "4S9BasU9Y9AchV2LaxvasITakPnwVyZ1";
        var payload = new (string, string)[]
        {
            ("UserId", "megaUser"),
            ("SessionId", "curentSessionId")
        };
        var expireIn = DateTime.Parse("3000-09-29T00:00:00.0000000+05:00");
        var expectedToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJVc2VySWQiOiJtZWdhVXNlciIsIlNlc3Npb25JZCI6ImN1cmVudFNlc3Npb25JZCIsImV4cCI6MzI1MjcwNzY0MDB9.kMM_-WfZPevJeQVAy6WRd2AQLHhu4uN4_5C8lPIBxMQ";

        var token = jwtProvider.GenerateToken(payload, secretKey, expireIn);

        Assert.Equal(expectedToken, token);
    }

    [Fact]
    public void VerifyValidTokenToken()
    {
        var jwtProvider = new JwtProvider();
        var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJVc2VySWQiOiIyOTk1OWQ4NS04MTllLTRiNDEtYmY0Yi01ZDk1YzVjZTg1ZGQiLCJleHAiOjc4Mjc5NTY0MDN9.LN__MbM5sHK6-uOflHqfGzTV7MMXQ3NcS7LftlEPcKY";
        var secretKey = "MYsecretKey123secretk3asedgsedgsdgY";

        var verified = jwtProvider.VerifyToken(token, secretKey, out (string, string)[] payload);

        Assert.True(verified);
        Assert.NotNull(payload);
        Assert.NotEmpty(payload);
        Assert.Equal("UserId", payload[0].Item1);
        Assert.Equal("29959d85-819e-4b41-bf4b-5d95c5ce85dd", payload[0].Item2);
    }

    [Fact]
    public void VerifyExpiredToken()
    {
        var jwtProvider = new JwtProvider();
        var expiredToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJVc2VySWQiOiIyOTk1OWQ4NS04MTllLTRiNDEtYmY0Yi01ZDk1YzVjZTg1ZGQiLCJleHAiOjE2NzI1NTY0MDB9.-8hskryeR84n8cpzUACMSsfwi6h6PpzDs4HbnuIZJqc";
        var secretKey = "lw4arXKWh8prVc6ewygQIPBKjBEvTfnP";

        var verified = jwtProvider.VerifyToken(expiredToken, secretKey, out (string, string)[] payload);

        Assert.False(verified);
        Assert.Null(payload);
    }

    [Fact]
    public void VerifyChangedToken()
    {
        var jwtProvider = new JwtProvider();
        var changedToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJVc2VySWQiOiIyOTk1OWQ4NS04MTllLTRiNDEtYmY0Yi01ZDk1YzVjZTg1ZGQiLCJleHAiOjczMjI5NDY1MjAwfQ.imGdCJPoV-s0ZUjzo1wDAQezMNdDcVZRwypv5zhgTFs";
        var secretKey = "lw4arXKWh8prVc6ewygQIPBKjBEvTfnP";

        var verified = jwtProvider.VerifyToken(changedToken, secretKey, out (string, string)[] payload);

        Assert.False(verified);
        Assert.Null(payload);
    }

    [Fact]
    public void VerifyTokenWithWrongSecretKey()
    {
        var jwtProvider = new JwtProvider();
        var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJVc2VySWQiOiJ1c2VySWQiLCJleHAiOjQ4MjgyMTU5NjAwfQ.7htlmhBfDFE1X0-V49ChDGxb3PZ6ynjGKoCpReL-s60";
        var wrongSecretKey = "wrongsecretkey";

        var verified = jwtProvider.VerifyToken(token, wrongSecretKey, out (string, string)[] payload);

        Assert.False(verified);
        Assert.Null(payload);
    }
}
