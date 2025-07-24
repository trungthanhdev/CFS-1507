using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CFS_1507.Contract.DTOs.ProductDto.Request;
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
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace CFS_1507.Application.Usecases.ProductUC.Commands
{
    public class UpdateProductCommand(string product_id, ReqUpdateProductDto arg) : IRequest<OkResponse>
    {
        public string Product_id = product_id;
        public ReqUpdateProductDto Arg = arg;
    }
    public class UpdateProductCommandHandler(
        IUnitOfWork unitOfWork,
        AppDbContext dbContext,
        UserIdentifyService userIdentify,
        ICheckInstanceOfTEntityClass<ProductEntity> productCheck,
        ICheckInstanceOfTEntityClass<UserEntity> userCheck,
        ICheckInstanceOfTEntityClass<TranslateEntity> productTranslateCheck

    ) : IRequestHandler<UpdateProductCommand, OkResponse>
    {
        public async Task<OkResponse> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var user_id = userIdentify.GetUserId();
            var currentUser = await dbContext.UserEntities
                .AsNoTracking()
                .Where(x => x.user_id == user_id)
                .FirstOrDefaultAsync(cancellationToken);
            currentUser = userCheck.CheckNullOrNot(currentUser, "Current user");

            var currentProduct = await dbContext.ProductEntities
                .Where(x => x.product_id == request.Product_id)
                .FirstOrDefaultAsync(cancellationToken);
            currentProduct = productCheck.CheckNullOrNot(currentProduct, "Current product");

            var currentProductTranslate = await dbContext.TranslateEntities
                .Where(x => x.product_id == request.Product_id)
                .FirstOrDefaultAsync(cancellationToken);
            currentProductTranslate = productTranslateCheck.CheckNullOrNot(currentProductTranslate, "Current product translate");

            if (string.IsNullOrWhiteSpace(request.Arg.translate_description) && request.Arg.product_description != null)
            {
                request.Arg.translate_description = request.Arg.product_description + "default!";
            }
            currentUser.UpdateProduct(request.Arg, currentProduct);
            currentUser.UpdateProductTranslate(request.Arg, currentProductTranslate);

            if (await unitOfWork.SaveChangeAsync(cancellationToken) > 0)
            {
                return new OkResponse($"User {currentUser.userName} update product {currentProduct.product_id} successfully!");
            }
            throw new InvalidOperationException("Fail to update product, nothing changes!");
        }
    }
}