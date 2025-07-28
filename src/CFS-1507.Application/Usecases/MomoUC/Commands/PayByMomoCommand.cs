using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CFS_1507.Contract.DTOs.CartDto.Request;
using CFS_1507.Contract.Helper;
using CFS_1507.Domain.Entities;
using CFS_1507.Domain.Interfaces;
using CFS_1507.Infrastructure.Integrations;
using CFS_1507.Infrastructure.Persistence;
using CTCore.DynamicQuery.Common.Exceptions;
using CTCore.DynamicQuery.Core.Domain.Interfaces;
using CTCore.DynamicQuery.Core.Primitives;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CFS_1507.Application.Usecases.MomoUC.Commands
{
    //Purpose: prepare for payment process and hold related cart items.
    //Step:
    //1:  Get list of cart_items from client and Validate (in dto).
    //2: Authenticate user.
    //2.1: create temp_cart_id => save to db (CartEntity)
    //3: Loop through each cart item:
    //3.1: Mark status as IN_PROCESS.
    //3.2: Calculate total amount.
    //3.3: create momoPayload =>  temp_cart_id(orderID), amount(amount), orderInfo(request.Arg.OrderInfo) 
    //4: Lock cart and Call MomoService.CreatePaymentAsync.
    //5: Save DB via IUnitOfWork.

    public class PayByMomoCommand(ReqChooseCartItemsDto arg) : IRequest<string>
    {
        public ReqChooseCartItemsDto Arg = arg;
    }
    public class PayByMomoCommandHandler(
        MomoService momoService,
        AppDbContext dbContext,
        UserIdentifyService userIdentifyService,
        IUnitOfWork unitOfWork
    ) : IRequestHandler<PayByMomoCommand, string>
    {
        public async Task<string> Handle(PayByMomoCommand request, CancellationToken cancellationToken)
        {
            var dto = request.Arg;
            var now = DateTimeOffset.UtcNow;
            //1:
            if (dto.listCartItems is null || !dto.listCartItems.Any())
                throw new BadHttpRequestException("List cart items is null or empty!");
            if (string.IsNullOrEmpty(dto.CartId) || string.IsNullOrWhiteSpace(dto.CartId))
                throw new BadHttpRequestException("CartId is requestId!");
            //2:
            var user_id = userIdentifyService.GetUserId();
            double amount = 0;

            var currentCart = await dbContext.CartEntities
                .Where(x => x.cart_id == dto.CartId && x.is_Paid == false && x.user_id == user_id)
                .FirstOrDefaultAsync(cancellationToken);
            if (currentCart is null)
                throw new NotFoundException("Current cart not found!");
            if (currentCart.is_locked == true && currentCart.locked_at.HasValue && now - currentCart.locked_at < TimeSpan.FromMinutes(10))
            {
                var timePassed = now - currentCart.locked_at.Value;
                var timeRemaining = TimeSpan.FromMinutes(10) - timePassed;
                var minutes = timeRemaining.Minutes;
                var seconds = timeRemaining.Seconds;

                throw new BadHttpRequestException($"Cart is in process, try again after {minutes} minutes {seconds} seconds!");
            }
            else
            {
                currentCart.UnLockCart();
            }
            //2.1:
            currentCart.UpdateTempCartId();
            //3:
            foreach (var item in dto.listCartItems)
            {
                var cartItem = await dbContext.CartItemsEntities
                    .Where(x => x.product_id == item.product_id && x.cart_id == currentCart.cart_id)
                    .FirstOrDefaultAsync(cancellationToken);
                if (cartItem is null) throw new NotFoundException($"Product {item.product_id} not found!");
                //3.1:
                currentCart.ChangeStatusToInProcess(cartItem);
                //3.2:
                var totalItemPrice = cartItem.product_price * item.quantity;
                amount += totalItemPrice;
            }
            //3.3:
            // System.Console.WriteLine($"temp_cart_id: {currentCart.temp_cart_id}");
            var newOrderModel = new OrderInfoModel
            {
                CartId = currentCart.temp_cart_id,
                Amount = amount,
                OrderInfo = dto.OrderInfo
            };
            //4:
            currentCart.LockCart();
            var createMomoMethod = await momoService.CreatePaymentAsync(newOrderModel);
            if (createMomoMethod is null)
            {
                currentCart.UnLockCart();
                throw new InvalidOperationException("Can not handle data from momo!");
            }
            if (await unitOfWork.SaveChangeAsync(cancellationToken) > 0)
            {
                return createMomoMethod;
            }
            await unitOfWork.RollbackAsync();
            throw new InvalidOperationException("Fail to save, nothing changes!");
        }
    }
}

