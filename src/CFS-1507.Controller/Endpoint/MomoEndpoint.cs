using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CFS_1507.Application.Usecases.CartUC.Commands;
using CFS_1507.Application.Usecases.MomoUC.Commands;
using CFS_1507.Application.Usecases.OrderUC.Commands;
using CFS_1507.Contract.DTOs.CartDto.Request;
using CFS_1507.Contract.DTOs.MomoDto.Request;
using CFS_1507.Domain.Interfaces;
using CFS_1507.Infrastructure.Hubs;
using CFS_1507.Infrastructure.Integrations;
using CFS_1507.Infrastructure.Interfaces;
using CTCore.DynamicQuery.Common.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

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
           [FromServices] IRabbitMQPublisher publisher,
           [FromServices] IHubContext<MomoHub> momoHub,
           [FromServices] IMediator mediator)
        {
            try
            {
                try
                {
                    //1: parse cart_id from Guid(N) => normal Guid("D)
                    if (string.IsNullOrWhiteSpace(arg.OrderId))
                        throw new BadHttpRequestException("Invalid cart_id format.");
                    var temp_cart_id = Guid.ParseExact(arg.OrderId, "N").ToString("D");
                    //2: if success => call ordersuccessfullycommand
                    if (arg.ResultCode == 0)
                    {
                        // var result = await mediator.Send(new OrderSuccessfullyCommand(temp_cart_id));
                        await publisher.Handle(temp_cart_id);
                        // await momoHub.NotifyPurchaseSuccessfully(data.user_cart_id, "Success");
                        // await momoHub.Clients.Group(result.user_cart_id).SendAsync("PurchaseSuccessfully", result.user_cart_id, "Success");
                        return Results.Ok(new { success = true, arg.Message });
                    }
                    if (arg.ResultCode == 1006)
                    {
                        await publisher.Handle(temp_cart_id);
                        return Results.Ok(new { success = true, arg.Message });

                        // // await momoHub.NotifyPurchaseSuccessfully(data.user_cart_id, "Cancelled");

                        // var result = await mediator.Send(new RejectMomoPaymentCommand(temp_cart_id));
                        // System.Console.WriteLine($"statuscode: {arg.ResultCode}, msg: {arg.Message}");
                        // await momoHub.Clients.Group(result.user_cart_id).SendAsync("UserCancelOrder", result.user_cart_id, "UserCancelOrder");
                        // return Results.Ok(new { success = true, msg = result.msg });
                    }

                    return Results.Problem("Unsuccessfully!");
                }
                catch (System.Exception ex)
                {
                    System.Console.WriteLine($"Can not parse orderId from momo handle ipn or {ex.Message}");
                    return Results.Problem(ex.Message, statusCode: 500);
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