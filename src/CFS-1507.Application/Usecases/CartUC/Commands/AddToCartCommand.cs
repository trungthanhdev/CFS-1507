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

            var allProducts = await dbContext.ProductEntities
                .Select(x => new
                {
                    x.product_id,
                    x.product_name
                })
                .ToListAsync(cancellationToken);
            Dictionary<string, string> allProductDict = new Dictionary<string, string>();
            foreach (var item in allProducts)
            {
                if (!allProductDict.ContainsKey(item.product_id))
                {
                    allProductDict[item.product_id] = item.product_name ?? "default product";
                }
            }

            List<ListCartItems> listCartItems = new List<ListCartItems>();
            foreach (var item in dto)
            {
                if (allProductDict.ContainsKey(item.product_id))
                {
                    var currentProduct = await dbContext.ProductEntities
                        .Where(x => x.product_id == item.product_id)
                        .FirstOrDefaultAsync(cancellationToken);
                    if (currentProduct is null)
                        throw new NotFoundException($"Product {item.product_id} not found!");

                    var newListCartItem = new ListCartItems
                    {
                        product_id = item.product_id,
                        product_name = allProductDict[item.product_id],
                        quantity = item.quantity,
                        Product = currentProduct
                    };
                    listCartItems.Add(newListCartItem);
                }
            }
            currentUser.AddToCart(listCartItems);
            if (await unitOfWork.SaveChangeAsync(cancellationToken) > 0)
            {
                return new OkResponse($"User {currentUser.userName} adds items to cart successfully!");
            }
            throw new InvalidOperationException("Fail to save, nothing changes!");
        }
    }
}