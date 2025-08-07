using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CFS_1507.Contract.DTOs.ProductDto.Request;
using CFS_1507.Contract.DTOs.ProductDto.Response;
using CFS_1507.Domain.Entities;
using CFS_1507.Domain.Interfaces;
using CFS_1507.Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CFS_1507.Application.Usecases.ProductUC.Queries
{
    public record SearchProductQuery(string? lan, string? product_name) : IRequest<List<ResProductDto>> { }
    public class SearchProductQueryHandler(
        AppDbContext dbContext,
        ICheckInstanceOfTEntityClass<LanguageEntity> checkLanguage
    ) : IRequestHandler<SearchProductQuery, List<ResProductDto>>
    {
        public async Task<List<ResProductDto>> Handle(SearchProductQuery request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.lan))
                throw new BadHttpRequestException("Language is required!");
            if (string.IsNullOrEmpty(request.product_name))
                throw new BadHttpRequestException("Product name is required!");
            var lowerProductname = request.product_name.Trim().ToUpperInvariant();

            var currentLanguage = await dbContext.LanguageEntities.Where(x => x.language_code == request.lan).FirstOrDefaultAsync(cancellationToken);
            currentLanguage = checkLanguage.CheckNullOrNot(currentLanguage, "Language");

            List<ResProductDto> response = new List<ResProductDto>();
            switch (currentLanguage.language_code)
            {
                case "vi-VN":
                    response = await dbContext.ProductEntities
                    .Where(x => x.normalizedName != null && EF.Functions.ILike(x.normalizedName, $"%{lowerProductname}%"))
                    .Select(x => new ResProductDto
                    {
                        product_id = x.product_id,
                        translate_id = currentLanguage.language_id,
                        product_name = x.product_name,
                        product_price = x.product_price,
                        product_image = x.product_image,
                        product_description = x.product_description
                    })
                    .ToListAsync(cancellationToken);
                    break;
                case "en-US":
                    response = await dbContext.TranslateEntities
                    .Where(x => x.normalizedName != null && EF.Functions.ILike(x.normalizedName, $"%{lowerProductname}%"))
                    .Select(x => new ResProductDto
                    {
                        product_id = x.product_id,
                        translate_id = currentLanguage.language_id,
                        product_name = x.translate_name,
                        product_price = x.translate_price,
                        product_image = x.translate_image,
                        product_description = x.translate_image
                    })
                    .ToListAsync(cancellationToken);
                    break;
                default:
                    throw new BadHttpRequestException($"Language '{currentLanguage.language_code}' is not supported.");
            }

            return response;
        }
    }
}