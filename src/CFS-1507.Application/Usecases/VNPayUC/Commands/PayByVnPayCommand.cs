using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CFS_1507.Infrastructure.Integrations.Payment;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace CFS_1507.Application.Usecases.VNPayUC.Commands
{
    public record PayByVnPayCommand(OrderInfo order) : IRequest<string> { }
    public class PayByVnPayCommandHandler(
        VNPayService vnPayService,
        ILogger<PayByVnPayCommand> _logger
    ) : IRequestHandler<PayByVnPayCommand, string>
    {
        public Task<string> Handle(PayByVnPayCommand request, CancellationToken cancellationToken)
        {
            var model = request.order;
            if (model.OrderId is null || model.Amount <= 0)
            {
                throw new BadHttpRequestException("OrderID(cartId) is required!");
            }
            if (model.Amount <= 0)
            {
                throw new BadHttpRequestException("Amount must be greater than 0!");
            }
            var url = vnPayService.CreateVNPay(model);
            _logger.LogInformation($"URL in PayByVnPayCommand: {url}");
            return Task.FromResult(url);
        }
    }
}