using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CFS_1507.Application.Usecases.OrderUC.Commands;
using CFS_1507.Domain.Interfaces;
using CFS_1507.Infrastructure.Hubs;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CFS_1507.Infrastructure.Integrations.RabbitMQ
{
    public class RabbitMQConsumer : IHostedService
    {
        private readonly ILogger<RabbitMQConsumer> _logger;
        private readonly IConnection _conn;
        private readonly IHubContext<MomoHub> _momoHub;
        private readonly IServiceScopeFactory _scopeFactory;
        private IChannel? _ch;
        public RabbitMQConsumer(ILogger<RabbitMQConsumer> logger, IConnection connection, IServiceScopeFactory scopeFactory, IHubContext<MomoHub> momoHub)
        {
            _logger = logger;
            _conn = connection;
            _scopeFactory = scopeFactory;
            _momoHub = momoHub;
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var routingKey = "Momo.routing.key";
            var exchangeName = "MomoPayExchange";
            var queueName = "MomoPayQueue";

            _ch = await _conn.CreateChannelAsync();

            await _ch.ExchangeDeclareAsync(exchangeName, ExchangeType.Direct, durable: true, autoDelete: false);
            await _ch.QueueDeclareAsync(queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
            await _ch.QueueBindAsync(queueName, exchangeName, routingKey);

            var consumer = new AsyncEventingBasicConsumer(_ch);

            await _ch.BasicQosAsync(0u, prefetchCount: 1, false);

            consumer.ReceivedAsync += async (model, ea) =>
            {
                // var temp_cart_id = Encoding.UTF8.GetString(ea.Body.ToArray());
                var temp_cart_id = JsonSerializer.Deserialize<string>(ea.Body.Span)!;
                var deliveryTag = ea.DeliveryTag;

                try
                {
                    _logger.LogInformation($"CustomerQueueService consuming {queueName}");
                    using var scope = _scopeFactory.CreateScope();
                    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                    var results = await mediator.Send(new OrderSuccessfullyCommand(temp_cart_id));
                    if (results.msg == "Order successfully!")
                    {
                        await _ch.BasicAckAsync(deliveryTag, false);
                        await _momoHub.Clients.Group(results.user_cart_id).SendAsync("PurchaseSuccessfully", results.user_cart_id, "Success");
                        _logger.LogInformation("Acked message for temp_cart_id={TempCartId}", temp_cart_id);
                    }
                    else
                    {
                        await _ch.BasicNackAsync(deliveryTag, false, false);
                        _logger.LogWarning("Nacked(no requeue) temp_cart_id={TempCartId} due to business failure", temp_cart_id);
                    }
                }
                catch (System.Exception ex)
                {
                    if (!ea.Redelivered) // llan dau fail
                    {
                        await _ch.BasicNackAsync(ea.DeliveryTag, false, true); // requeue 1 lan nua
                        _logger.LogError(ex, "Transient DB error, requeued once. id={TempCartId}", temp_cart_id);
                    }
                    else
                    {
                        await _ch.BasicNackAsync(ea.DeliveryTag, false, false);
                        _logger.LogError(ex, "Repeated DB error, dead-lettered. id={TempCartId}", temp_cart_id);
                    }
                }
            };
            await _ch.BasicConsumeAsync(queueName, false, consumer);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _ch!.CloseAsync(cancellationToken); _ch = null;
        }
    }
}