using CFS_1507.Application;
using CFS_1507.Infrastructure;

namespace CFS_1507.Injection;

public static class Injection
{
    public static void AddInjection(this IServiceCollection services, IConfiguration configuration)
    {
        services.InjectApplication();
        services.AddInfrastructure(configuration);
    }
}