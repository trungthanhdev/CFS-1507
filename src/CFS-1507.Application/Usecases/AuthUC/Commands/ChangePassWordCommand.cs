using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CFS_1507.Contract.DTOs.AuthDto.Request;
using CFS_1507.Domain.Entities;
using CFS_1507.Domain.Interfaces;
using CFS_1507.Infrastructure.Persistence;
using CTCore.DynamicQuery.Common.Exceptions;
using CTCore.DynamicQuery.Core.Domain.Interfaces;
using CTCore.DynamicQuery.Core.Primitives;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CFS_1507.Application.Usecases.AuthUC.Commands
{
    public class ChangePassWordCommand(ReqChangePassDto arg) : IRequest<OkResponse>
    {
        public ReqChangePassDto Arg = arg;
    }
    public class ChangePassWordCommandHandler(
        IHttpContextAccessor httpContextAccessor,
        AppDbContext dbContext,
        IUnitOfWork unitOfWork,
        ICheckInstanceOfTEntityClass<UserEntity> userCheck
    ) : IRequestHandler<ChangePassWordCommand, OkResponse>
    {
        public async Task<OkResponse> Handle(ChangePassWordCommand request, CancellationToken cancellationToken)
        {
            var user_id = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (user_id is null) throw new NotFoundException("User_id from token not found!");

            var currentUser = await dbContext.UserEntities.Where(x => x.user_id == user_id).FirstOrDefaultAsync(cancellationToken);
            currentUser = userCheck.CheckNullOrNot(currentUser, "Current user");

            var checkPass = currentUser.CheckPassword(request.Arg.old_pass);
            if (checkPass == false) throw new UnauthorizedAccessException("Old password invalid!");

            currentUser.ChangePassword(request.Arg.new_pass);

            if (await unitOfWork.SaveChangeAsync(cancellationToken) > 0)
            {
                return new OkResponse($"User {currentUser.userName} updated password successfully!");
            }
            throw new InvalidOperationException("Change password failed, cannot save to database.");
        }
    }
}