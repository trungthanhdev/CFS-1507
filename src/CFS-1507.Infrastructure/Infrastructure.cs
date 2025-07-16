using CFS_1507.Domain.Interfaces;
using CFS_1507.Infrastructure.Persistence;
using CFS_1507.Infrastructure.Persistence.Repositories;
using CTCore.DynamicQuery.Core.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CFS_1507.Infrastructure;

public static class InfrastructureInjection
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        DotNetEnv.Env.Load();
        var opts = new DbContextOptionsBuilder();
        var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");

        if (string.IsNullOrEmpty(connectionString)) throw new InvalidOperationException("CONNECTION_STRING environment variable is not set.");

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped(typeof(IRepositoryDefinition<>), typeof(RepositoryDefinition<>));
        services.AddScoped<IUnitOfWork, UnitOfWork<AppDbContext>>();
    }
}
