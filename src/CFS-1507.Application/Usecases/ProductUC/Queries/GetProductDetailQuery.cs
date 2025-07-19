using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CFS_1507.Contract.DTOs.ProductDto.Response;
using CFS_1507.Infrastructure.Persistence;
using CTCore.DynamicQuery.Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CFS_1507.Application.Usecases.ProductUC.Queries
{
    public class GetProductDetailQuery(string product_id) : IRequest<ResProductDto>
    {
        public string Product_id = product_id;
    }
    public class GetProductDetailQueryHandler(
        AppDbContext dbContext
    ) : IRequestHandler<GetProductDetailQuery, ResProductDto>
    {
        public async Task<ResProductDto> Handle(GetProductDetailQuery request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Product_id))
                throw new ArgumentException("Product_id is required");

            var currentProduct = await dbContext.ProductEntities
            .Where(x => x.product_id == request.Product_id)
            .Select(x => new ResProductDto
            {
                product_id = x.product_id,
                product_name = x.product_name,
                product_image = x.product_image,
                product_price = x.product_price
            })
            .FirstOrDefaultAsync(cancellationToken);

            if (currentProduct is null)
                throw new NotFoundException("Product not found!");

            return currentProduct;
        }
    }
}