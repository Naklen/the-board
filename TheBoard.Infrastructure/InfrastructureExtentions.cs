using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TheBoard.Application.Auth;
using TheBoard.Application.Stores;
using TheBoard.Infrastructure.Auth;
using TheBoard.Infrastructure.Persistence;
using TheBoard.Infrastructure.Persistence.Repositories;

namespace TheBoard.Infrastructure;

public static class InfrastructureExtentions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IUserRepository, UserRepository>();

        services.AddScoped<IPasswordHasher, PasswordHasher>();

        services.AddDbContext<TheBoardDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString(nameof(TheBoardDbContext))));

        return services;
    }
}
