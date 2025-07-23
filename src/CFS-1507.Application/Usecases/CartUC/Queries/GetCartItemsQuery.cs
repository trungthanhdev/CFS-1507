using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CFS_1507.Contract.DTOs.CartDto.Reponse;
using CFS_1507.Contract.DTOs.CartDto.Request;
using CFS_1507.Contract.Helper;
using CFS_1507.Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CFS_1507.Application.Usecases.CartUC.Queries
{
    public record GetCartItemsQuery(QueryHelperDto dto) : IRequest<PageResponseDto<ResCartDto>> { }
    public class GetCartItemsQueryHandler(
        AppDbContext dbContext,
        UserIdentifyService userIdentifyService
    ) : IRequestHandler<GetCartItemsQuery, PageResponseDto<ResCartDto>>
    {
        public async Task<PageResponseDto<ResCartDto>> Handle(GetCartItemsQuery request, CancellationToken cancellationToken)
        {
            var user_id = userIdentifyService.GetUserId();
            var currentUser = await userIdentifyService.FindCurrentUser(user_id);
            var dto = request.dto;
            if (dto.pageNumber <= 0 || dto.pageSize <= 0)
                throw new BadHttpRequestException("Page size and page number are greater than 0");
            var currentCart = await dbContext.CartEntities
                .Where(x => x.user_id == user_id && x.is_Paid == false)
                .FirstOrDefaultAsync(cancellationToken);
            if (currentCart is null)
                return new PageResponseDto<ResCartDto>
                {
                    pageNumber = dto.pageNumber,
                    pageSize = dto.pageSize,
                    total = 0,
                    data = []
                };
            var skip = (dto.pageNumber - 1) * dto.pageSize;
            var take = dto.pageSize;

            var allCartItemsQuery = dbContext.CartItemsEntities
                .Where(x => x.cart_id == currentCart.cart_id)
                .OrderBy(x => x.created_at);
            var total = await allCartItemsQuery.CountAsync();
            var allCartItems = await allCartItemsQuery
                .Include(x => x.Product)
                .Skip(skip)
                .Take(take)
                .Select(x => new ResCartItemsDto
                {
                    cart_item_id = x.cart_item_id,
                    quantity = x.quantity,
                    product_name = x.Product!.product_name
                })
                .ToListAsync(cancellationToken);

            var response = new ResCartDto
            {
                cart_id = currentCart.cart_id,
                cartItems = allCartItems
            };
            return new PageResponseDto<ResCartDto>
            {
                pageNumber = dto.pageNumber,
                pageSize = dto.pageSize,
                total = total,
                data = [response]
            };
        }
    }
}