using Microsoft.Extensions.DependencyInjection;
using TheBoard.Application.Services;

namespace TheBoard.Application;

public static class ApplicationExtentions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<UserService>();
        services.AddScoped<TokenService>();

        return services;
    }
}
