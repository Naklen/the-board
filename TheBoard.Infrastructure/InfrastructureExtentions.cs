using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using TheBoard.Application.Auth;
using TheBoard.Application.Interfaces;
using TheBoard.Application.Stores;
using TheBoard.Infrastructure.Auth;
using TheBoard.Infrastructure.Cache;
using TheBoard.Infrastructure.Persistence;
using TheBoard.Infrastructure.Persistence.Repositories;

namespace TheBoard.Infrastructure;

public static class InfrastructureExtentions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IUserRepository, UserRepository>();

        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IJwtProvider, JwtProvider>();

        services.AddDbContext<TheBoardDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString(nameof(TheBoardDbContext))));

        services.AddScoped<ITokenCacheStorage, TokenCahceStorage>();

        services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(GetEnvironmentVariable("REDIS_COFIGURATION")));

        return services;
    }
}
