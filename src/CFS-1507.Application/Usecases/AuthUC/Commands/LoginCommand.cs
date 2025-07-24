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
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CFS_1507.Application.Usecases.AuthUC.Commands
{
    public class LoginCommand(ReqLoginDto arg) : IRequest<ResponseAuthDto>
    {
        public ReqLoginDto Arg = arg;
    }
    public class LoginCommandHandler(
        AppDbContext dbContext,
        ICheckInstanceOfTEntityClass<UserEntity> userCheck,
        TokenService tokenService,
        ICheckInstanceOfTEntityClass<AttachToEntity> attachToCheck,
        ICheckInstanceOfTEntityClass<RoleEntity> roleCheck
    ) : IRequestHandler<LoginCommand, ResponseAuthDto>
    {
        public async Task<ResponseAuthDto> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Arg.username) || string.IsNullOrWhiteSpace(request.Arg.password))
                throw new BadHttpRequestException("User name and Password are required!");

            var currentUser = await dbContext.UserEntities.Where(x => x.userName == request.Arg.username).FirstOrDefaultAsync(cancellationToken);
            currentUser = userCheck.CheckNullOrNot(currentUser, "Current user");

            var is_Verified = currentUser.CheckPassword(request.Arg.password);
            if (is_Verified == false)
                throw new UnauthorizedAccessException("Password invalid!");

            var userRole = await dbContext.AttachToEntities.Where(x => x.user_id == currentUser.user_id).FirstOrDefaultAsync(cancellationToken);
            userRole = attachToCheck.CheckNullOrNot(userRole, "Role attached");
            var userRoleName = await dbContext.RoleEntities.Where(x => x.role_id == userRole.role_id).FirstOrDefaultAsync(cancellationToken);
            userRoleName = roleCheck.CheckNullOrNot(userRoleName, "Role");

            var access_token = await tokenService.GenerateToken(currentUser, true);
            var refresh_token = await tokenService.GenerateToken(currentUser, false);
            return new ResponseAuthDto { access_token = access_token, refresh_token = refresh_token, user_name = currentUser.userName, user_id = currentUser.user_id, role = userRoleName.role_name };
        }
    }
}