using TheBoard.Application.Contracts;
using TheBoard.Application.Services;
using TheBoard.Infrastructure.Auth;

namespace TheBoard.Tests.Application;

public class TokenServiceTests
{
    [Fact]
    public void GetTokenPayloadTest()
    {
        var service = new TokenService(new JwtProvider(), null);
        var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.OJjPOfUx2i3fdDKY267W1c5qLGwgQHFyekDgKjVxeqc";
        var expected = new List<TokenPayloadField>
            {
                new ("sub", "1234567890"),
                new ("name", "John Doe"),
                new ("iat", "1516239022")
            };

        var actual = service.GetTokenPayload(token);

        Assert.Equal(expected, actual);
    }
}
