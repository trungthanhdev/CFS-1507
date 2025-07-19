using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using CFS_1507.Contract.DTOs.ProductDto.Response;
using CFS_1507.Contract.Helper;
using CFS_1507.Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CFS_1507.Application.Usecases.ProductUC.Queries
{
    public class GetAllProductQuery(QueryHelperDto helper) : IRequest<PageResponseDto<ResProductDto>>
    {
        public QueryHelperDto Helpler = helper;
    }
    public class GetAllProductQueryHadndler(
        AppDbContext dbContext
    ) : IRequestHandler<GetAllProductQuery, PageResponseDto<ResProductDto>>
    {
        public async Task<PageResponseDto<ResProductDto>> Handle(GetAllProductQuery request, CancellationToken cancellationToken)
        {
            var helper = request.Helpler;
            if (helper.pageNumber <= 0 || helper.pageSize <= 0)
                throw new BadHttpRequestException("Page size and Page number are required!");
            var allProduct = dbContext.ProductEntities
                .Select(x => new ResProductDto
                {
                    product_id = x.product_id,
                    product_name = x.product_name,
                    product_price = x.product_price,
                    product_image = x.product_image
                });
            var total = allProduct.Count();
            if (helper.sortByPrice == true)
                allProduct = allProduct.OrderByDescending(x => x.product_price);
            else
                allProduct = allProduct.OrderBy(x => x.product_id);

            var skip = (helper.pageNumber - 1) * helper.pageSize;
            var take = helper.pageSize;

            var data = await allProduct
                .Skip(skip)
                .Take(take)
                .ToListAsync();
            return new PageResponseDto<ResProductDto>
            {
                pageNumber = helper.pageNumber,
                pageSize = helper.pageSize,
                total = total,
                sortByPrice = helper.sortByPrice,
                data = data
            };
        }
    }
}