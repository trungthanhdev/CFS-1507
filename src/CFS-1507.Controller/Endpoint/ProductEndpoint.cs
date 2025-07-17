using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CFS_1507.Application.Usecases.ProductUC.Commands;
using CFS_1507.Contract.DTOs.ProductDto.Request;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CFS_1507.Controller.Endpoint
{
    public class ProductEndpoint
    {
        public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
        {
            var pr = endpoints
                .MapGroup("api/v1/product")
                .WithDisplayName("Product");

            pr.MapPost("/create", CreateProduct).RequireAuthorization().DisableAntiforgery();
            return endpoints;
        }
        public async Task<IResult> CreateProduct(
            [FromForm] ReqCreateProductDto arg,
            [FromServices] IMediator mediator)
        {
            try
            {
                var result = await mediator.Send(new CreateProductCommand(arg));
                return Results.Ok(new { success = 200, data = result });
            }
            catch (BadHttpRequestException ex)
            {
                return Results.Problem(ex.Message, statusCode: 400);
            }
            catch (InvalidOperationException ex)
            {
                return Results.Problem(ex.Message, statusCode: 500);
            }
        }
    }
}