using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CFS_1507.Application.Usecases.ProductUC.Commands;
using CFS_1507.Application.Usecases.ProductUC.Queries;
using CFS_1507.Contract.DTOs.ProductDto.Request;
using CFS_1507.Contract.Helper;
using CFS_1507.Domain.Common;
using CTCore.DynamicQuery.Common.Exceptions;
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

            pr.MapPost("/create", CreateProduct).RequireAuthorization(ERole.ADMIN.ToString()).DisableAntiforgery();
            pr.MapGet("/get-all", GetAllProduct);
            pr.MapGet("/{product_id}", GetProductDetail);
            pr.MapPatch("/{product_id}", SoftDeleteProduct).RequireAuthorization(ERole.ADMIN.ToString());
            pr.MapPatch("/update/{product_id}", UpdateProduct).RequireAuthorization(ERole.ADMIN.ToString()).DisableAntiforgery();
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

        public async Task<IResult> GetAllProduct(
            [FromQuery] int? pageNumber,
            [FromQuery] int? pageSize,
            [FromQuery] bool? sortByPrice,
            [FromQuery] string? lan,
            [FromServices] IMediator mediator)
        {
            try
            {
                var queryHelper = new QueryHelperDto
                {
                    pageNumber = pageNumber ?? 0,
                    pageSize = pageSize ?? 0,
                    sortByPrice = sortByPrice ?? false
                };
                var result = await mediator.Send(new GetAllProductQuery(queryHelper, lan));
                return Results.Ok(new { success = 200, result });
            }
            catch (BadHttpRequestException ex)
            {
                return Results.Problem(ex.Message, statusCode: 400);
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message, statusCode: 500);
            }
        }

        public async Task<IResult> GetProductDetail(
            [FromRoute] string? product_id,
            [FromQuery] string? lan,
            [FromServices] IMediator mediator)
        {
            try
            {
                var result = await mediator.Send(new GetProductDetailQuery(product_id, lan));
                return Results.Ok(new { success = 200, data = result });
            }
            catch (NotFoundException ex)
            {
                return Results.Problem(ex.Message, statusCode: 404);
            }
            catch (BadHttpRequestException ex)
            {
                return Results.Problem(ex.Message, statusCode: 400);
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message, statusCode: 500);
            }
        }
        public async Task<IResult> SoftDeleteProduct(
            [FromRoute] string product_id,
            [FromServices] IMediator mediator)
        {
            try
            {
                var result = await mediator.Send(new SoftDeleteProductCommand(product_id));
                return Results.Ok(new { success = 200, data = result });
            }
            catch (NotFoundException ex)
            {
                return Results.Problem(ex.Message, statusCode: 400);
            }
            catch (InvalidOperationException ex)
            {
                return Results.Problem(ex.Message, statusCode: 500);
            }
        }
        public async Task<IResult> UpdateProduct(
            [FromRoute] string product_id,
            [FromForm] ReqUpdateProductDto dto,
            [FromServices] IMediator mediator)
        {
            try
            {
                var result = await mediator.Send(new UpdateProductCommand(product_id, dto));
                return Results.Ok(new { success = 200, data = result });
            }
            catch (NotFoundException ex)
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