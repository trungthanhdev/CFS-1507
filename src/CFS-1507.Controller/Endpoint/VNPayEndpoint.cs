using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CFS_1507.Application.Usecases.VNPayUC.Commands;
using CFS_1507.Infrastructure.Integrations.Payment;
using CTCore.DynamicQuery.Common.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CFS_1507.Controller.Endpoint
{
    public class VNPayEndpoint
    {
        public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
        {
            var pr = endpoints
                .MapGroup("api/v1/vnpay")
                .WithDisplayName("VNPay");

            pr.MapPost("/vnpay-payment", PayByVNPay).RequireAuthorization();
            // pr.MapPost("/handle-ipn", HandleIPN);
            return endpoints;
        }
        public async Task<IResult> PayByVNPay(
          [FromBody] OrderInfo arg,
          [FromServices] IMediator mediator)
        {
            try
            {
                var result = await mediator.Send(new PayByVnPayCommand(arg));
                return Results.Redirect(result);
                // return Results.Ok(new { success = 200, result });
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