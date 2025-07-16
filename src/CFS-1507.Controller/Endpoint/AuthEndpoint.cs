using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CFS_1507.Application.Usecases.AuthUC.Commands;
using CFS_1507.Contract.DTOs.AuthDto.Request;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CFS_1507.Controller.Endpoint
{
    public class AuthEndpoint
    {
        public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
        {
            var au = endpoints
                .MapGroup("api/v1/auth")
                .WithDisplayName("Auth");

            au.MapPost("/register", Register);

            return endpoints;
        }

        public async Task<IResult> Register(
            [FromBody] ReqRegisterDto arg,
            [FromServices] IMediator mediator)
        {
            try
            {
                var result = await mediator.Send(new RegisterCommand(arg));
                return Results.Ok(new { success = true, data = result });
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
    }
}