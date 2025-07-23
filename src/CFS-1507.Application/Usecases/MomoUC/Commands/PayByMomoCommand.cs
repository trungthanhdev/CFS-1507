using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CFS_1507.Infrastructure.Integrations;
using CTCore.DynamicQuery.Core.Primitives;
using MediatR;

namespace CFS_1507.Application.Usecases.MomoUC.Commands
{
    public class PayByMomoCommand(OrderInfoModel arg) : IRequest<object>
    {
        public OrderInfoModel Arg = arg;
    }
    public class PayByMomoCommandHandler(
        MomoService momoService
    ) : IRequestHandler<PayByMomoCommand, object>
    {
        public async Task<object> Handle(PayByMomoCommand request, CancellationToken cancellationToken)
        {
            var createMomoMethod = await momoService.CreatePaymentAsync(request.Arg);
            if (createMomoMethod is null)
                throw new InvalidOperationException("Can not handle data from momo!");
            return createMomoMethod;
        }
    }
}