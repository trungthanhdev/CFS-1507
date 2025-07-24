using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CFS_1507.Contract.Helper;
using CFS_1507.Domain.Entities;
using CFS_1507.Domain.Interfaces;
using CFS_1507.Infrastructure.Persistence;
using CTCore.DynamicQuery.Common.Exceptions;
using CTCore.DynamicQuery.Core.Domain.Interfaces;
using CTCore.DynamicQuery.Core.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CFS_1507.Application.Usecases.CartUC.Commands
{
    public class DeleteCartItemsCommand(string product_id) : IRequest<OkResponse>
    {
        public string Product_id = product_id;
    }
    public class DeleteCartItemsCommandHandler(
        AppDbContext dbContext,
        UserIdentifyService userIdentifyService,
        IRepositoryDefinition<CartItemsEntity> cartItemsRepo,
        IUnitOfWork unitOfWork,
        ICheckInstanceOfTEntityClass<CartEntity> cartCheck,
        ICheckInstanceOfTEntityClass<CartItemsEntity> cartItemCheck,
        ICheckInstanceOfTEntityClass<ProductEntity> productCheck
    ) : IRequestHandler<DeleteCartItemsCommand, OkResponse>
    {
        public async Task<OkResponse> Handle(DeleteCartItemsCommand request, CancellationToken cancellationToken)
        {
            var user_id = userIdentifyService.GetUserId();
            var currentUser = await userIdentifyService.FindCurrentUser(user_id);
            var existedCart = await dbContext.CartEntities
                .Include(x => x.CartItemsEntities)
                .Where(x => x.user_id == user_id && x.is_Paid == false)
                .FirstOrDefaultAsync(cancellationToken);
            existedCart = cartCheck.CheckNullOrNot(existedCart, "Current cart");

            var currentProduct = existedCart.CartItemsEntities.FirstOrDefault(x => x.product_id == request.Product_id);
            currentProduct = cartItemCheck.CheckNullOrNot(currentProduct, "Current cart item");

            cartItemsRepo.Remove(new List<CartItemsEntity> { currentProduct });
            var product = await dbContext.ProductEntities
                    .Where(x => x.product_id == request.Product_id)
                    .FirstOrDefaultAsync(cancellationToken);
            product = productCheck.CheckNullOrNot(product, "Current product");
            product.UndoIsInCart();
            if (await unitOfWork.SaveChangeAsync(cancellationToken) > 0)
            {
                return new OkResponse($"User {currentUser.userName} deleted successfully!");
            }
            throw new InvalidOperationException("Fail to save, nothing changes!");
        }
    }
}