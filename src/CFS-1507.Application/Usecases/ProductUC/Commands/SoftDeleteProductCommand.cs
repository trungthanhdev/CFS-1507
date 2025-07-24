using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CFS_1507.Contract.Helper;
using CFS_1507.Domain.Entities;
using CFS_1507.Domain.Interfaces;
using CFS_1507.Infrastructure.Persistence;
using CTCore.DynamicQuery.Common.Exceptions;
using CTCore.DynamicQuery.Core.Domain.Interfaces;
using CTCore.DynamicQuery.Core.Primitives;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CFS_1507.Application.Usecases.ProductUC.Commands
{
    public class SoftDeleteProductCommand(string product_id) : IRequest<OkResponse>
    {
        public string Product_id = product_id;
    }
    public class SoftDeleteProductCommandHandler(
        IUnitOfWork unitOfWork,
        AppDbContext dbContext,
        UserIdentifyService userIdentify,
        ICheckInstanceOfTEntityClass<ProductEntity> productCheck,
        ICheckInstanceOfTEntityClass<UserEntity> userCheck
    ) : IRequestHandler<SoftDeleteProductCommand, OkResponse>
    {
        public async Task<OkResponse> Handle(SoftDeleteProductCommand request, CancellationToken cancellationToken)
        {
            var user_id = userIdentify.GetUserId();
            var currentProduct = await dbContext.ProductEntities.Where(x => x.product_id == request.Product_id).FirstOrDefaultAsync(cancellationToken);
            currentProduct = productCheck.CheckNullOrNot(currentProduct, "Current product");

            var currentUser = await dbContext.UserEntities.Where(x => x.user_id == user_id).FirstOrDefaultAsync(cancellationToken);
            currentUser = userCheck.CheckNullOrNot(currentUser, "Current user");

            currentUser.ToggleDeleteProduct(currentProduct);
            if (await unitOfWork.SaveChangeAsync(cancellationToken) > 0)
            {
                return new OkResponse($"User {currentUser.userName} deleted product {currentProduct.product_name} successfully!");
            }
            throw new InvalidOperationException("Fail to delete, nothing changes!");
        }
    }
}