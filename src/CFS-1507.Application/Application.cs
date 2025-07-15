using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace CFS_1507.Application;

public static class ApplicationInjection
{
    public static void InjectApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

    }
}
