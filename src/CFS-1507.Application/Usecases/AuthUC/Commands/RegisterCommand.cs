using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CFS_1507.Application.Services.TokenService;
using CFS_1507.Contract.DTOs.AuthDto.Request;
using CFS_1507.Contract.DTOs.AuthDto.Response;
using CFS_1507.Domain.Entities;
using CFS_1507.Domain.Interfaces;
using CFS_1507.Infrastructure.Persistence;
using CTCore.DynamicQuery.Core.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CFS_1507.Application.Usecases.AuthUC.Commands
{
    public class RegisterCommand(ReqRegisterDto arg) : IRequest<ResponseAuthDto>
    {
        public ReqRegisterDto Arg = arg;
    }
    public class RegisterCommandHandler(
        IRepositoryDefinition<UserEntity> userRepo,
        AppDbContext dbContext,
        IUnitOfWork unitOfWork,
        TokenService tokenService
    ) : IRequestHandler<RegisterCommand, ResponseAuthDto>
    {
        public async Task<ResponseAuthDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Arg.username) || string.IsNullOrWhiteSpace(request.Arg.password))
                throw new BadHttpRequestException("User name and Password are required!");

            var existedUser = await dbContext.UserEntities.Where(x => x.userName == request.Arg.username).FirstOrDefaultAsync(cancellationToken);
            if (existedUser != null) throw new UnauthorizedAccessException("Account already existed!");

            var newUser = UserEntity.Create(request.Arg);
            userRepo.Add(new List<UserEntity> { newUser });

            var role_id = await dbContext.RoleEntities.Where(x => x.role_name == request.Arg.role).Select(x => x.role_id).FirstOrDefaultAsync(cancellationToken);
            if (role_id is null) throw new BadHttpRequestException("Role not found!");

            newUser.AddRole(role_id);
            if (await unitOfWork.SaveChangeAsync(cancellationToken) > 0)
            {
                var accessToken = await tokenService.GenerateToken(newUser, true);
                var refreshToken = await tokenService.GenerateToken(newUser, false);
                return new ResponseAuthDto { access_token = accessToken, refresh_token = refreshToken, user_name = newUser.userName, user_id = newUser.user_id };
            }
            throw new InvalidOperationException("Register failed, cannot save to database.");
        }
    }
}