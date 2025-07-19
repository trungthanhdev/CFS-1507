using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CFS_1507.Contract.DTOs.ProductDto.Request;
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
        IHttpContextAccessor contextAccessor
    ) : IRequestHandler<UpdateProductCommand, OkResponse>
    {
        public async Task<OkResponse> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var user_id = contextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(user_id))
                throw new NotFoundException("User_id from token not found!");

            var currentUser = await dbContext.UserEntities
                .AsNoTracking()
                .Where(x => x.user_id == user_id)
                .FirstOrDefaultAsync(cancellationToken);
            if (currentUser is null)
                throw new NotFoundException("Current user not found!");

            var currentProduct = await dbContext.ProductEntities
                .Where(x => x.product_id == request.Product_id)
                .FirstOrDefaultAsync(cancellationToken);
            if (currentProduct is null)
                throw new NotFoundException("Current product not found!");

            currentUser.UpdateProduct(request.Arg, currentProduct);
            if (await unitOfWork.SaveChangeAsync(cancellationToken) > 0)
            {
                return new OkResponse($"User {currentUser.userName} update product {currentProduct.product_id} successfully!");
            }
            throw new InvalidOperationException("Fail to update product, nothing changes!");
        }
    }
}