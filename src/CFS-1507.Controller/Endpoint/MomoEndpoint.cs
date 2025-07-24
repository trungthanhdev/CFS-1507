using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CFS_1507.Application.Usecases.MomoUC.Commands;
using CFS_1507.Application.Usecases.OrderUC.Commands;
using CFS_1507.Contract.DTOs.CartDto.Request;
using CFS_1507.Contract.DTOs.MomoDto.Request;
using CFS_1507.Infrastructure.Integrations;
using CTCore.DynamicQuery.Common.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CFS_1507.Controller.Endpoint
{
    public class MomoEndpoint
    {
        public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
        {
            var pr = endpoints
                .MapGroup("api/v1/momo")
                .WithDisplayName("Momo");

            pr.MapPost("/momo-payment", PayByMomo).RequireAuthorization();
            pr.MapPost("/handle-ipn", HandleIPN);
            return endpoints;
        }
        public async Task<IResult> PayByMomo(
           [FromBody] ReqChooseCartItemsDto arg,
           [FromServices] IMediator mediator)
        {
            try
            {
                var result = await mediator.Send(new PayByMomoCommand(arg));
                return Results.Ok(new { success = 200, result });
            }
            catch (BadHttpRequestException ex)
            {
                return Results.Problem(ex.Message, statusCode: 400);
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
        public async Task<IResult> HandleIPN(
           [FromBody] MomoIPNDto arg,
           [FromServices] IMediator mediator)
        {
            try
            {
                try
                {
                    //1: parse cart_id from Guid(N) => normal Guid
                    if (string.IsNullOrWhiteSpace(arg.OrderId))
                        throw new BadHttpRequestException("Invalid cart_id format.");
                    var cart_id = Guid.ParseExact(arg.OrderId, "N").ToString("D");
                    //2: if success => call ordersuccessfullycommand
                    if (arg.ResultCode == 0)
                    {
                        var result = await mediator.Send(new OrderSuccessfullyCommand(cart_id));
                        return Results.Ok(new { success = true, msg = result });
                    }
                    if (arg.ResultCode == 1006)
                    {
                        var result = await mediator.Send(new OrderSuccessfullyCommand(cart_id));// dung de test chuc nang thanh toan thanh cong
                        System.Console.WriteLine($"statuscode: {arg.ResultCode}, msg: {arg.Message}");
                        return Results.Ok(new { success = true, msg = "User canceled!" });
                    }

                    return Results.Problem("Unsuccessfully!");
                }
                catch (System.Exception)
                {
                    System.Console.WriteLine("Can not parse orderId from momo handle ipn");
                    throw;
                }
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