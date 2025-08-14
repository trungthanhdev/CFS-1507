using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CFS_1507.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System.Text.Json;
namespace CFS_1507.Infrastructure.Integrations.RabbitMQ
{
    public class RabbitMQPublish : IRabbitMQPublisher
    {
        private readonly IConnection _conn;
        public readonly ILogger<RabbitMQPublish> _logger;
        public RabbitMQPublish(IConnection connection, ILogger<RabbitMQPublish> logger)
        {
            _conn = connection;
            _logger = logger;
        }
        public async Task Handle(string temp_cart_id)
        {
            var routingKey = "Momo.routing.key";
            var exchangeName = "MomoPayExchange";
            var queueName = "MomoPayQueue";

            var channel = await _conn.CreateChannelAsync();
            await channel.ExchangeDeclareAsync(exchangeName, ExchangeType.Direct, true, false);
            await channel.QueueDeclareAsync(queueName, true, false, false, null);
            await channel.QueueBindAsync(queueName, exchangeName, routingKey);

            var props = new BasicProperties
            {
                ContentType = "application/json",
                DeliveryMode = DeliveryModes.Persistent,
                MessageId = temp_cart_id,
            };
            var payloadToByte = JsonSerializer.SerializeToUtf8Bytes(temp_cart_id);
            await channel.BasicPublishAsync(exchange: exchangeName, routingKey: routingKey, mandatory: false, basicProperties: props, body: payloadToByte);
            _logger.LogInformation($"Added temp_cart_id {temp_cart_id} to queue!");
        }
    }
}