using System.Threading.Tasks;
using CFS_1507.Domain.Common;
using CFS_1507.Domain.Interfaces;
using CFS_1507.Infrastructure.Helper;
using CFS_1507.Infrastructure.Hubs;
using CFS_1507.Infrastructure.Integrations;
using CFS_1507.Infrastructure.Integrations.Payment;
using CFS_1507.Infrastructure.Integrations.RabbitMQ;
using CFS_1507.Infrastructure.Interfaces;
using CFS_1507.Infrastructure.Persistence;
using CFS_1507.Infrastructure.Persistence.Repositories;
using CTCore.DynamicQuery.Core.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace CFS_1507.Infrastructure;

public static class InfrastructureInjection
{
    public static async Task AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        DotNetEnv.Env.Load();
        var opts = new DbContextOptionsBuilder();
        var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");

        if (string.IsNullOrEmpty(connectionString)) throw new InvalidOperationException("CONNECTION_STRING environment variable is not set.");

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddHttpContextAccessor();
        services.AddScoped(typeof(IRepositoryDefinition<>), typeof(RepositoryDefinition<>));
        services.AddScoped<IUnitOfWork, UnitOfWork<AppDbContext>>();
        services.AddScoped<ILocalStorage, LocalStorage>();
        services.AddScoped<VnPayLibrary>();
        services.AddScoped<MomoService>();
        services.AddScoped<VNPayService>();
        services.AddScoped(typeof(ICheckInstanceOfTEntityClass<>), typeof(CheckInstanceOfTEntityClass<>));
        // services.AddScoped<IMomoHub, MomoHub>();

        //RabbitMQ
        var hostName = Environment.GetEnvironmentVariable("HOST_NAME_RBMQ") ?? throw new InvalidOperationException("HOST_NAME_RBMQ not found!");
        var port = Environment.GetEnvironmentVariable("PORT_RBMQ") ?? throw new InvalidOperationException("PORT_RBMQ not found!");
        if (!int.TryParse(port, out var portNumber))
        {
            throw new InvalidOperationException("Can not parse port RabbitMQ!");
        }
        var userName = Environment.GetEnvironmentVariable("USER_NAME_RBMQ") ?? throw new InvalidOperationException("USER_NAME_RBMQ not found!");
        var passWord = Environment.GetEnvironmentVariable("PASS_RBMQ") ?? throw new InvalidOperationException("PASS_RBMQ not found!");
        var factory = new ConnectionFactory()
        {
            HostName = hostName,
            Port = portNumber,
            UserName = userName,
            Password = passWord,
            VirtualHost = "/",
            AutomaticRecoveryEnabled = true,
            TopologyRecoveryEnabled = true,
        };
        var connection = await factory.CreateConnectionAsync();
        services.AddSingleton<IConnection>(connection);
        services.AddScoped<IRabbitMQPublisher, RabbitMQPublish>();
    }
}
