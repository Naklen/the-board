namespace TheBoard.API.Middlewares;

public class AccessTokenMiddleware
{
    private readonly RequestDelegate _next;

    public AccessTokenMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var token = context.Request.Cookies[GetEnvironmentVariable("JWT_ACCESS_COOKIE_NAME")];
        if (!string.IsNullOrEmpty(token))
            context.Request.Headers.Append("Authorization", "Bearer " + token);

        context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
        context.Response.Headers.Append("X-Xss-Protection", "1");
        context.Response.Headers.Append("X-Frame-Options", "DENY");

        await _next(context);
    }
}

public static class AccessTokenMiddlewareExtentions
{
    public static IApplicationBuilder UseAccessToken(this IApplicationBuilder app)
    {
        return app.UseMiddleware<AccessTokenMiddleware>();
    }
}
