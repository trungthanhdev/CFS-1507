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
    //Purpose: confirm successful payment and convert cart items to order and orderItems.
    //step:
    //1: Find cart based on cart_id.
    //2: Filter cart items with status == "IN_PROCESS".
    //3: Create OrderEntity and corresponding OrderItemsEntity.
    //4: Check how many cartItem "PENDING" in cart? if == 0 => updateCartStatus
    //5: Save to DB via unitOfWork.

    public class OrderSuccessfullyCommand(string cart_id) : IRequest<OkResponse>
    {
        public string Cart_id = cart_id;
    }
    public class OrderSuccessfullyCommandHandler(
        AppDbContext dbContext,
        IUnitOfWork unitOfWork,
        IRepositoryDefinition<OrderEntity> orderRepo,
        IRepositoryDefinition<OrderItemsEntity> orderItemsRepo
    ) : IRequestHandler<OrderSuccessfullyCommand, OkResponse>
    {
        public async Task<OkResponse> Handle(OrderSuccessfullyCommand request, CancellationToken cancellationToken)
        {
            //1:
            var currentCart = await dbContext.CartEntities
                .Include(x => x.CartItemsEntities)
                .Where(x => x.cart_id == request.Cart_id)
                .FirstOrDefaultAsync(cancellationToken);
            if (currentCart is null)
                throw new NotFoundException("Current cart not found!");
            //2:
            var cartItemList = await dbContext.CartItemsEntities
                .Where(x => x.cart_id == currentCart.cart_id && x.status == "IN_PROCESS")
                .ToListAsync(cancellationToken);
            //3:
            var newOrder = currentCart.CreateOrder(currentCart.cart_id, currentCart.user_id);
            List<OrderItemsEntity> orderItemList = new List<OrderItemsEntity>();
            foreach (var item in cartItemList)
            {
                currentCart.ChangeStatusToCompleted(item);
                var newOrderItem = newOrder.AddToOrderItems(newOrder.order_id, item.product_id, item.quantity);
                orderItemList.Add(newOrderItem);
            }
            orderRepo.Add(new List<OrderEntity> { newOrder });
            orderItemsRepo.Add(orderItemList);
            //4:
            var stillPendingItems = await dbContext.CartItemsEntities
                .Where(x => x.cart_id == currentCart.cart_id && x.status == "PENDING")
                .AnyAsync(cancellationToken);

            if (!stillPendingItems)
            {
                currentCart.CartPaid();
            }
            //5:
            if (await unitOfWork.SaveChangeAsync(cancellationToken) > 0)
            {
                return new OkResponse("Order successfully!");
            }
            throw new InvalidOperationException("Fail to save, nothing changes!");
        }
    }
}