using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CFS_1507.Domain.Entities;
using CFS_1507.Domain.Interfaces;
using CFS_1507.Infrastructure.Persistence;
using CTCore.DynamicQuery.Common.Exceptions;
using CTCore.DynamicQuery.Core.Domain.Interfaces;
using CTCore.DynamicQuery.Core.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CFS_1507.Application.Usecases.OrderUC.Commands
{
    public class OrderSuccessfullyCommand(string cart_id) : IRequest<OkResponse>
    {
        public string Cart_id = cart_id;
    }
    public class OrderSuccessfullyCommandHandler(
        AppDbContext dbContext,
        IUnitOfWork unitOfWork,
        IRepositoryDefinition<OrderEntity> orderRepo,
        IRepositoryDefinition<OrderItemsEntity> orderItemRepo
    ) : IRequestHandler<OrderSuccessfullyCommand, OkResponse>
    {
        public async Task<OkResponse> Handle(OrderSuccessfullyCommand request, CancellationToken cancellationToken)
        {
            var currentCart = await dbContext.CartEntities
                .Include(x => x.CartItemsEntities)
                .Where(x => x.cart_id == request.Cart_id)
                .FirstOrDefaultAsync(cancellationToken);
            if (currentCart is null)
                throw new NotFoundException("Current cart not found!");
            var newOrder = currentCart.CreateOrder(currentCart.cart_id, currentCart.user_id);
            orderRepo.Add(new List<OrderEntity> { newOrder });
            foreach (var item in currentCart.CartItemsEntities)
            {
                var newOrderItem = newOrder.AddToOrderItems(newOrder.order_id, item.product_id, item.quantity);
                orderItemRepo.Add(new List<OrderItemsEntity> { newOrderItem });
            }
            currentCart.CartPaid();
            if (await unitOfWork.SaveChangeAsync(cancellationToken) > 0)
            {
                return new OkResponse("Order successfully!");
            }
            throw new InvalidOperationException("Fail to save, nothing changes!");
        }
    }
}