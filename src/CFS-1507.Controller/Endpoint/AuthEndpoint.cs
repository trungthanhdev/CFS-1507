using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CFS_1507.Application.Usecases.AuthUC.Commands;
using CFS_1507.Contract.DTOs.AuthDto.Request;
using CFS_1507.Domain.Common;
using CTCore.DynamicQuery.Common.Exceptions;
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
            au.MapPost("/log-in", Login);
            au.MapPost("/log-out", LogOut).RequireAuthorization();
            au.MapPatch("/change-password", ChangePassword).RequireAuthorization();

            return endpoints;
        }

        public async Task<IResult> Register(
            [FromBody] ReqRegisterDto arg,
            [FromServices] IMediator mediator)
        {
            try
            {
                var result = await mediator.Send(new RegisterCommand(arg));
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

        public async Task<IResult> Login(
            [FromBody] ReqLoginDto arg,
            [FromServices] IMediator mediator)
        {
            try
            {
                var result = await mediator.Send(new LoginCommand(arg));
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
            catch (Exception ex)
            {
                return Results.Problem(ex.Message, statusCode: 500);
            }
        }

        public async Task<IResult> ChangePassword(
            [FromBody] ReqChangePassDto arg,
            [FromServices] IMediator mediator)
        {
            try
            {
                var result = await mediator.Send(new ChangePassWordCommand(arg));
                return Results.Ok(new { success = 200, data = result });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Results.Problem(ex.Message, statusCode: 400);
            }
            catch (NotFoundException ex)
            {
                return Results.Problem(ex.Message, statusCode: 401);
            }
            catch (InvalidOperationException ex)
            {
                return Results.Problem(ex.Message, statusCode: 500);
            }
        }

        public async Task<IResult> LogOut(
            [FromServices] IMediator mediator)
        {
            try
            {
                var result = await mediator.Send(new LogOutCommand());
                return Results.Ok(new { success = 200, data = result });
            }
            catch (NotFoundException ex)
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