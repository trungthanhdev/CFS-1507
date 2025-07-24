using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CFS_1507.Application.Usecases.CategoryUC.Commands;
using CFS_1507.Application.Usecases.CategoryUC.Queries;
using CFS_1507.Contract.DTOs.CateDto.Request;
using CFS_1507.Domain.Common;
using CTCore.DynamicQuery.Common.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CFS_1507.Controller.Endpoint
{
    public class CategoryEndpoint
    {
        public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
        {
            var pr = endpoints
                .MapGroup("api/v1/category")
                .WithDisplayName("Category");

            pr.MapPost("/", CreateCategory).RequireAuthorization(ERole.ADMIN.ToString());
            pr.MapGet("/", GetAllCategories).RequireAuthorization(ERole.ADMIN.ToString());
            pr.MapPatch("/{category_id}", UpdateCategory).RequireAuthorization(ERole.ADMIN.ToString());
            return endpoints;
        }
        public async Task<IResult> CreateCategory(
            [FromBody] ReqCreateCategoryDto arg,
            [FromServices] IMediator mediator)
        {
            try
            {
                var result = await mediator.Send(new CreateCategoryCommand(arg));
                return Results.Ok(new { success = 200, data = result });
            }
            catch (BadHttpRequestException ex)
            {
                return Results.Problem(ex.Message, statusCode: 400);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Results.Problem(ex.Message, statusCode: 401);
            }
            catch (InvalidOperationException ex)
            {
                return Results.Problem(ex.Message, statusCode: 500);
            }
        }
        public async Task<IResult> GetAllCategories(
            [FromServices] IMediator mediator)
        {
            try
            {
                var result = await mediator.Send(new GetAllCategoryQuery());
                return Results.Ok(new { success = 200, data = result });
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message, statusCode: 500);
            }
        }
        public async Task<IResult> UpdateCategory(
            [FromBody] ReqUpdateCategoryDto dto,
            [FromRoute] string category_id,
            [FromServices] IMediator mediator)
        {
            try
            {
                var result = await mediator.Send(new UpdateCategoryCommand(category_id, dto));
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
            catch (InvalidOperationException ex)
            {
                return Results.Problem(ex.Message, statusCode: 500);
            }
        }
    }
}