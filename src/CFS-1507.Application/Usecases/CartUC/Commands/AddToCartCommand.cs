using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CFS_1507.Contract.DTOs.CartDto.Request;
using CFS_1507.Contract.Helper;
using CFS_1507.Domain.DTOs;
using CFS_1507.Domain.Entities;
using CFS_1507.Infrastructure.Persistence;
using CTCore.DynamicQuery.Common.Exceptions;
using CTCore.DynamicQuery.Core.Domain.Interfaces;
using CTCore.DynamicQuery.Core.Primitives;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CFS_1507.Application.Usecases.CartUC.Commands
{
    public class AddToCartCommand(ReqAddToCartDto arg) : IRequest<OkResponse>
    {
        public ReqAddToCartDto Arg = arg;
    }
    public class AddToCartCommandHandler(
        IUnitOfWork unitOfWork,
        AppDbContext dbContext,
        UserIdentifyService userIdentifyService
    ) : IRequestHandler<AddToCartCommand, OkResponse>
    {
        public async Task<OkResponse> Handle(AddToCartCommand request, CancellationToken cancellationToken)
        {
            var dto = request.Arg.listCartItems;
            var user_id = userIdentifyService.GetUserId();
            var currentUser = await userIdentifyService.FindCurrentUser(user_id);
            if (dto is null || !dto.Any())
                throw new BadHttpRequestException("Cart items are required!");

            var allProductsDict = await dbContext.ProductEntities
                .ToDictionaryAsync(x => x.product_id, x => x, cancellationToken);


            List<ListCartItems> listCartItems = new List<ListCartItems>();
            foreach (var item in dto)
            {
                if (allProductsDict.TryGetValue(item.product_id, out var currentProduct))
                {
                    var newListCartItem = new ListCartItems
                    {
                        product_id = item.product_id,
                        product_name = currentProduct.product_name ?? "default product",
                        quantity = item.quantity,
                        product_price = item.product_price,
                        Product = currentProduct
                    };
                    listCartItems.Add(newListCartItem);
                }
                else
                {
                    throw new NotFoundException($"Product {item.product_id} not found!");
                }
            }

            var existedCart = await dbContext.CartEntities
                .Include(x => x.CartItemsEntities)
                .Where(x => x.user_id == user_id && x.is_Paid == false)
                .FirstOrDefaultAsync(cancellationToken);
            if (existedCart is null)
            {
                currentUser.AddToCart(listCartItems);
            }
            else
            {
                foreach (var item in listCartItems)
                {
                    currentUser.AddCartItem(existedCart, item);
                }
            }

            if (await unitOfWork.SaveChangeAsync(cancellationToken) > 0)
            {
                return new OkResponse($"User {currentUser.userName} adds items to cart successfully!");
            }
            throw new InvalidOperationException("Fail to save, nothing changes!");
        }
    }
}