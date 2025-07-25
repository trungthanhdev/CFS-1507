using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CFS_1507.Contract.DTOs.ProductDto.Response;
using CFS_1507.Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CFS_1507.Application.Usecases.ProductUC.Queries
{
    // 1: get all category_id and product_id type Dictionary => CateDict[category_id] : List<product_id> (.distinct().tolist())
    // 2: get category name (category_name) base on category_id in CateDict
    // 3: get list of all product_id from value in CateDict (CateDict[category_id].Value)
    // 4: get product information base on listProductId and specified language (request.lan)
    // 5: map response dto
    public record GetAllProductPageQuery(string? lan) : IRequest<List<ResProductPageDto>> { }
    public class GetAllProductPageQueryHandler(
        AppDbContext dbContext
    ) : IRequestHandler<GetAllProductPageQuery, List<ResProductPageDto>>
    {
        public async Task<List<ResProductPageDto>> Handle(GetAllProductPageQuery request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.lan))
                throw new BadHttpRequestException("Language is required!");
            //1:
            var cateDict = await dbContext.ProductCateEntities
            .GroupBy(x => x.category_id)
            .ToDictionaryAsync(x => x.Key,
                               x => x.Select(x => x.product_id).Distinct().ToList(),
                               cancellationToken);
            //2:
            var cateName = await dbContext.CategoryEntities
            .Where(x => cateDict.Keys.Contains(x.category_id))
            .ToListAsync(cancellationToken);
            //3:
            List<string> listProductIds = new List<string>();
            foreach (var item in cateDict.Keys)
            {
                var product = cateDict.TryGetValue(item, out var productIds);
                if (product == true)
                {
                    foreach (var proIdItem in productIds!)
                    {
                        listProductIds.Add(proIdItem);
                    }
                }
            }
            //4: 
            List<ResProductItemDto> productDetailList;
            switch (request.lan)
            {
                case "vi-VN":
                    productDetailList = await dbContext.ProductEntities
                                  .Where(x => listProductIds.Contains(x.product_id))
                                   .Select(p => new ResProductItemDto
                                   {
                                       product_id = p.product_id,
                                       product_name = p.product_name,
                                       product_price = p.product_price,
                                       product_description = p.product_description
                                   })
                                  .ToListAsync(cancellationToken);

                    break;
                case "en-US":
                    productDetailList = await dbContext.TranslateEntities
                                  .Where(x => listProductIds.Contains(x.product_id))
                                   .Select(p => new ResProductItemDto
                                   {
                                       product_id = p.product_id,
                                       product_name = p.translate_name,
                                       product_price = p.translate_price,
                                       product_description = p.translate_description
                                   })
                                  .ToListAsync(cancellationToken);
                    break;
                default:
                    throw new BadHttpRequestException("Language code invalid!");
            }
            //5:
            var response = new List<ResProductPageDto>();
            foreach (var category in cateDict)
            {
                var categoryId = category.Key;
                var productIds = category.Value;

                var categoryEntity = cateName.FirstOrDefault(x => x.category_id == categoryId);
                var categoryName = categoryEntity?.category_name ?? "Default!";

                var products = productDetailList
                    .Where(p => productIds.Contains(p.product_id!))
                    .Select(p => new ResProductItemDto
                    {
                        product_id = p.product_id,
                        product_name = p.product_name,
                        product_price = p.product_price,
                        product_description = p.product_description
                    })
                    .ToList();
                response.Add(new ResProductPageDto
                {
                    category_name = categoryName,
                    listProductItems = products
                });
            }
            return response;
        }
    }
}
