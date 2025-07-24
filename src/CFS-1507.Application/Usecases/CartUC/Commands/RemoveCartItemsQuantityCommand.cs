using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CFS_1507.Contract.DTOs.CartDto.Request;
using CFS_1507.Contract.Helper;
using CFS_1507.Domain.DTOs;
using CFS_1507.Domain.Entities;
using CFS_1507.Domain.Interfaces;
using CFS_1507.Infrastructure.Persistence;
using CTCore.DynamicQuery.Common.Exceptions;
using CTCore.DynamicQuery.Core.Domain.Interfaces;
using CTCore.DynamicQuery.Core.Primitives;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace CFS_1507.Application.Usecases.CartUC.Commands
{
    public class RemoveCartItemsQuantityCommand(ReqRemoveQuantityDto arg) : IRequest<OkResponse>
    {
        public ReqRemoveQuantityDto Arg = arg;
    }
    public class RemoveCartItemsQuantityCommandHandler(
        IUnitOfWork unitOfWork,
        UserIdentifyService userIdentifyService,
        AppDbContext dbContext,
        IRepositoryDefinition<CartItemsEntity> cartItemRepo,
        ICheckInstanceOfTEntityClass<CartEntity> cartCheck
    ) : IRequestHandler<RemoveCartItemsQuantityCommand, OkResponse>
    {
        public async Task<OkResponse> Handle(RemoveCartItemsQuantityCommand request, CancellationToken cancellationToken)
        {
            var user_id = userIdentifyService.GetUserId();
            var currentUser = await userIdentifyService.FindCurrentUser(user_id);

            var currentCart = await dbContext.CartEntities
                .Where(x => x.user_id == user_id && x.is_Paid == false)
                .FirstOrDefaultAsync(cancellationToken);
            currentCart = cartCheck.CheckNullOrNot(currentCart, "Current cart");

            if (request.Arg.listCartItems == null || !request.Arg.listCartItems.Any())
                throw new BadHttpRequestException("List product_id is null or empty!");
            var productIds = request.Arg.listCartItems
                .Select(x => x.product_id)
                .ToList();
            if (!productIds.Any())
                throw new BadHttpRequestException("List product_id is null!");

            var listCartItems = await dbContext.CartItemsEntities
                .Include(x => x.Product)
                .Where(x => x.cart_id == currentCart.cart_id && productIds.Contains(x.product_id))
                .ToListAsync(cancellationToken);

            var cartItemsDict = request.Arg.listCartItems
                .GroupBy(x => x.product_id)
                .ToDictionary(g => g.Key, g => g.Sum(x => x.quantity));
            foreach (var item in listCartItems)
            {
                currentUser.RemoveCartItemQuantity(currentCart, item, cartItemsDict[item.product_id]);
                if (item.quantity == 0)
                {
                    cartItemRepo.Remove(new List<CartItemsEntity> { item });
                }
            }

            if (await unitOfWork.SaveChangeAsync(cancellationToken) > 0)
            {
                return new OkResponse($"User {currentUser.userName} removed successfully!");
            }
            throw new InvalidOperationException("Fail to save, nothing changes!");
        }
    }
}