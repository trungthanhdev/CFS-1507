using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CFS_1507.Domain.Entities;
using CFS_1507.Domain.Interfaces;
using CTCore.DynamicQuery.Common.Exceptions;
using CTCore.DynamicQuery.Core.Domain.Interfaces;
using CTCore.DynamicQuery.Core.Primitives;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace CFS_1507.Application.Usecases.AuthUC.Commands
{
    public class LogOutCommand : IRequest<OkResponse>
    {
    }
    public class LogOutCommandHandler(
        IHttpContextAccessor httpContextAccessor,
        IUnitOfWork unitOfWork,
        IRepositoryDefinition<BlackListEntity> blackListRepo
    ) : IRequestHandler<LogOutCommand, OkResponse>
    {
        public async Task<OkResponse> Handle(LogOutCommand request, CancellationToken cancellationToken)
        {
            var token_id = httpContextAccessor.HttpContext?.User?.FindFirst("token_id")?.Value;
            var user_id = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var username = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrWhiteSpace(token_id) || string.IsNullOrWhiteSpace(user_id) || string.IsNullOrEmpty(username))
                throw new NotFoundException("token_id or user_id not or username found from token!");

            var newBlackList = BlackListEntity.Create(user_id, token_id);
            blackListRepo.Add(new List<BlackListEntity> { newBlackList });
            if (await unitOfWork.SaveChangeAsync(cancellationToken) > 0)
            {
                return new OkResponse($"User {username} logged out successfully!");
            }
            throw new InvalidOperationException("Logged out failed!");
        }
    }
}