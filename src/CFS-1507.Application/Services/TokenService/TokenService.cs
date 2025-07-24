using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using CFS_1507.Contract.DTOs.AuthDto.Response;
using CFS_1507.Domain.Entities;
using CFS_1507.Infrastructure.Persistence;
using DotNetEnv;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CFS_1507.Application.Services.TokenService
{
    public class TokenService(
        ILogger<TokenService> _logger,
        AppDbContext dbContext
    )
    {
        // validate token
        public IOptions<ClaimsPrincipal> ValidateToken(string token)
        {
            //1: init instace of JwtSecurityTokenHandler
            var tokenHandler = new JwtSecurityTokenHandler();

            //2: lay secret, Issuer, Audience trong env
            var tokenSecret = Environment.GetEnvironmentVariable("JWT_SECRET");
            var audience = Environment.GetEnvironmentVariable("Audience");
            var issuer = Environment.GetEnvironmentVariable("Issuer");
            if (tokenSecret is null) throw new InvalidOperationException("Token secret not found from ENV");
            if (audience is null) throw new InvalidOperationException("Audience not found from ENV");
            if (issuer is null) throw new InvalidOperationException("Issuer not found from ENV");

            //3: tao key, convert token secret tu string => byte[] 
            var key = Encoding.ASCII.GetBytes(tokenSecret);

            //3.1: cau hinh nhung yeu to can xac minh trong token
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = issuer,
                ValidateAudience = true,
                ValidAudience = audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
            //3: validate => isntance(JwtSecurityTokenHandler).Validate()
            try
            {
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out _);
                return Options.Create(principal);
            }
            catch (SecurityTokenExpiredException ex)
            {
                _logger.LogWarning("Token expired: " + ex.Message);
                throw new UnauthorizedAccessException("Token expired");
            }
            catch (Exception ex)
            {
                _logger.LogError("Token invalid: " + ex.Message);
                throw new UnauthorizedAccessException("Token invalid");
            }
        }

        // generate token (access, refresh)
        public async Task<string> GenerateToken(UserEntity user, bool is_Access)
        {
            //1: tao instance cua JwtSecurityTokenHandler
            var tokenHandler = new JwtSecurityTokenHandler();

            //2: lay cac key can thiet
            var tokenSecret = Environment.GetEnvironmentVariable("JWT_SECRET");
            var refreshSecret = Environment.GetEnvironmentVariable("JWT_REFRESH_SECRET");
            var accessExpired = Environment.GetEnvironmentVariable("JWT_ACCESS_EXPIRES_IN");
            var refreshExpired = Environment.GetEnvironmentVariable("JWT_REFRESH_EXPIRES_IN");
            var audience = Environment.GetEnvironmentVariable("Audience");
            var issuer = Environment.GetEnvironmentVariable("Issuer");
            if (tokenSecret is null) throw new InvalidOperationException("Token secret not found from ENV");
            if (refreshSecret is null) throw new InvalidOperationException("Refresh Token secret not found from ENV");
            if (accessExpired is null) throw new InvalidOperationException("Access Token expired not found from ENV");
            if (refreshExpired is null) throw new InvalidOperationException("Refresh Token expired not found from ENV");
            if (audience is null) throw new InvalidOperationException("Audience not found from ENV");
            if (issuer is null) throw new InvalidOperationException("Issuer not found from ENV");

            //3: convert token secret tu string => byte[] 
            var key = Encoding.ASCII.GetBytes(tokenSecret);
            var refreshToken = Encoding.ASCII.GetBytes(refreshSecret);

            //3.1: payload token
            string token;
            var dt = DateTime.UtcNow;

            if (!double.TryParse(accessExpired, out var accessTokenExpiresIn))
                throw new InvalidOperationException("Invalid access token expiration format");

            if (!double.TryParse(refreshExpired, out var refreshTokenExpiresIn))
                throw new InvalidOperationException("Invalid refresh token expiration format");

            var userRole = await dbContext.AttachToEntities
                .Include(x => x.Role)
                .Where(x => x.user_id == user.user_id && x.Role != null)
                .Select(x => x.Role!.role_name)
                .ToListAsync();

            var claims = new List<Claim>
                {
                    new Claim("token_id", Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.NameIdentifier, user.user_id),
                    new Claim(ClaimTypes.Name, user.userName),
                    new Claim(ClaimTypes.Email, user.email ?? "")
                };
            foreach (var role in userRole)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            //4: generate
            if (is_Access)
            {
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = dt.AddMinutes(accessTokenExpiresIn),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                    Issuer = issuer,
                    Audience = audience
                };
                var access = tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
                token = access;
            }
            else
            {
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = dt.AddMinutes(refreshTokenExpiresIn),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                    Issuer = issuer,
                    Audience = audience
                };
                var refresh = tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
                token = refresh;
            }
            return token;
        }
    }
}