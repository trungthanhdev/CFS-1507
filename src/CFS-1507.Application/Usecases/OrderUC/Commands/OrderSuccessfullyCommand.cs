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
    //3: Create OrderEntity and corresponding OrderItemsEntity and update product's is_bouhgt.
    //4: Check how many cartItem "PENDING" in cart? if == 0 => updateCartStatus
    //5: Save to DB via unitOfWork.

    public class OrderSuccessfullyCommand(string temp_cart_id) : IRequest<OkResponse>
    {
        public string Temp_cart_id = temp_cart_id;
    }
    public class OrderSuccessfullyCommandHandler(
        AppDbContext dbContext,
        IUnitOfWork unitOfWork,
        IRepositoryDefinition<OrderEntity> orderRepo,
        IRepositoryDefinition<OrderItemsEntity> orderItemsRepo,
        ICheckInstanceOfTEntityClass<CartEntity> cartCheck
    ) : IRequestHandler<OrderSuccessfullyCommand, OkResponse>
    {
        public async Task<OkResponse> Handle(OrderSuccessfullyCommand request, CancellationToken cancellationToken)
        {
            //1:
            var currentCart = await dbContext.CartEntities
                .Include(x => x.CartItemsEntities)
                .Where(x => x.temp_cart_id == request.Temp_cart_id)
                .FirstOrDefaultAsync(cancellationToken);
            currentCart = cartCheck.CheckNullOrNot(currentCart, "Current cart");
            //2:
            var cartItemList = await dbContext.CartItemsEntities
                .Include(x => x.Product)
                .Where(x => x.cart_id == currentCart.cart_id && x.status == "IN_PROCESS")
                .ToListAsync(cancellationToken);
            //3:
            var newOrder = currentCart.CreateOrder(currentCart.cart_id, currentCart.user_id);
            List<OrderItemsEntity> orderItemList = new List<OrderItemsEntity>();
            Dictionary<string, int> productIDDict = new Dictionary<string, int>();
            foreach (var item in cartItemList)
            {
                currentCart.ChangeStatusToCompleted(item);
                var newOrderItem = newOrder.AddToOrderItems(newOrder.order_id, item.product_id, item.quantity);
                orderItemList.Add(newOrderItem);
                productIDDict[item.product_id] = item.quantity;
                item.Product?.UndoIsInCart();
            }
            orderRepo.Add(new List<OrderEntity> { newOrder });
            orderItemsRepo.Add(orderItemList);
            var listProductBought = await dbContext.ProductEntities
                .Where(x => productIDDict.Keys.Contains(x.product_id))
                .ToListAsync(cancellationToken);
            foreach (var item in listProductBought)
            {
                item.InCreaseIsBought(productIDDict[item.product_id]);
            }
            ;
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