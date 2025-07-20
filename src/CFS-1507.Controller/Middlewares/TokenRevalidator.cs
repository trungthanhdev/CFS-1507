using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using CFS_1507.Application.Services.TokenService;
using CFS_1507.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace CFS_1507.Controller.Middlewares
{
    public class TokenRevalidator(
        TokenService _tokenService,
        AppDbContext dbContext,
        ILogger<TokenRevalidator> _logger) : IMiddleware
    {
        public async Task InvokeAsync(HttpContext httpContext, RequestDelegate next)
        {
            var endpoint = httpContext.GetEndpoint();
            var isAuthorizeEndpoint = endpoint?.Metadata.GetMetadata<AuthorizeAttribute>() != null;

            if (isAuthorizeEndpoint)
            {
                try
                {
                    // Extract token from Authorization header
                    var rawToken = ExtractToken(httpContext);

                    // validate token 
                    var usedToken = "";
                    if (!string.IsNullOrEmpty(rawToken))
                    {
                        var tokenValidator = _tokenService.ValidateToken(rawToken).Value;
                        httpContext.User = tokenValidator;

                        var jit = tokenValidator.Claims.FirstOrDefault(e => e.Type == "jti")?.Value;
                        var userId = tokenValidator.Claims.FirstOrDefault(e => e.Type == ClaimTypes.NameIdentifier)?.Value;
                        var token_id = tokenValidator.Claims.FirstOrDefault(e => e.Type == "token_id")?.Value ?? tokenValidator.Claims.FirstOrDefault(e => e.Type == "jti")?.Value;
                        var role = tokenValidator.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                        usedToken = token_id;
                    }

                    // check token in blacklist or not 
                    if (string.IsNullOrEmpty(usedToken))
                    {
                        throw new UnauthorizedAccessException("Invalid token.");
                    }
                    var tokenUsed = await dbContext.BlackListEntities.Where(x => x.token_id == usedToken).FirstOrDefaultAsync();
                    if (tokenUsed != null)
                    {
                        throw new UnauthorizedAccessException("Token is used!");
                    }

                    await next(httpContext);
                }
                catch (UnauthorizedAccessException ex)
                {
                    _logger.LogError(ex, "An exception occurred: {Message}", ex.Message);

                    httpContext.Response.ContentType = "application/json";
                    httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;

                    await using var writer = new Utf8JsonWriter(httpContext.Response.BodyWriter);
                    writer.WriteStartObject();
                    writer.WriteNumber("stCode", 401);
                    writer.WriteString("msg", "Auth failed");
                    writer.WriteEndObject();
                    await writer.FlushAsync();
                }
            }
            else
            {
                await next(httpContext);
            }
        }


        private string ExtractToken(HttpContext context)
        {
            if (!context.Request.Headers.TryGetValue("Authorization", out var authHeader))
            {
                return string.Empty;
            }

            var authHeaderValue = authHeader.ToString();

            // Check if it's a Bearer token
            if (!authHeaderValue.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                return string.Empty;
            }

            // Extract the token part (remove "Bearer " prefix)
            return authHeaderValue.Substring(7).Trim();
        }
    }
}