using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CFS_1507.Application.Usecases.CartUC.Commands;
using CFS_1507.Contract.DTOs.CartDto.Request;
using CTCore.DynamicQuery.Common.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CFS_1507.Controller.Endpoint
{
    public class CartEndpoint
    {
        public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
        {
            var pr = endpoints
                .MapGroup("api/v1/cart")
                .WithDisplayName("Cart");

            pr.MapPost("/add-to-cart", AddToCart).RequireAuthorization();
            pr.MapDelete("/{product_id}", DeleteCartItem).RequireAuthorization();
            pr.MapPatch("/remove-quantity", RemoveCartItemQuantity).RequireAuthorization();
            return endpoints;
        }
        public async Task<IResult> AddToCart(
            [FromBody] ReqAddToCartDto dto,
            [FromServices] IMediator mediator)
        {
            try
            {
                var result = await mediator.Send(new AddToCartCommand(dto));
                return Results.Ok(new { success = 200, data = result });
            }
            catch (BadHttpRequestException ex)
            {
                return Results.Problem(ex.Message, statusCode: 400);
            }
            catch (NotFoundException ex)
            {
                return Results.Problem(ex.Message, statusCode: 404);
            }
            catch (InvalidOperationException ex)
            {
                return Results.Problem(ex.Message, statusCode: 500);
            }
        }

        public async Task<IResult> DeleteCartItem(
            [FromRoute] string product_id,
            [FromServices] IMediator mediator)
        {
            try
            {
                var result = await mediator.Send(new DeleteCartItemsCommand(product_id));
                return Results.Ok(new { success = 200, data = result });
            }
            catch (BadHttpRequestException ex)
            {
                return Results.Problem(ex.Message, statusCode: 400);
            }
            catch (NotFoundException ex)
            {
                return Results.Problem(ex.Message, statusCode: 404);
            }
            catch (InvalidOperationException ex)
            {
                return Results.Problem(ex.Message, statusCode: 500);
            }
        }
        public async Task<IResult> RemoveCartItemQuantity(
            [FromBody] ReqRemoveQuantityDto dto,
            [FromServices] IMediator mediator)
        {
            try
            {
                var result = await mediator.Send(new RemoveCartItemsQuantityCommand(dto));
                return Results.Ok(new { success = 200, data = result });
            }
            catch (BadHttpRequestException ex)
            {
                return Results.Problem(ex.Message, statusCode: 400);
            }
            catch (NotFoundException ex)
            {
                return Results.Problem(ex.Message, statusCode: 404);
            }
            catch (InvalidOperationException ex)
            {
                return Results.Problem(ex.Message, statusCode: 500);
            }
        }
    }
}