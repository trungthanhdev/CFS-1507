using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CFS_1507.Contract.DTOs.CateDto.Request;
using CFS_1507.Contract.Helper;
using CFS_1507.Domain.Entities;
using CFS_1507.Domain.Interfaces;
using CFS_1507.Infrastructure.Persistence;
using CTCore.DynamicQuery.Core.Domain.Interfaces;
using CTCore.DynamicQuery.Core.Primitives;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CFS_1507.Application.Usecases.CategoryUC.Commands
{
    public record UpdateCategoryCommand(string category_id, ReqUpdateCategoryDto dto) : IRequest<OkResponse> { }
    public class UpdateCategoryCommandHandler(
        IUnitOfWork unitOfWork,
        AppDbContext dbContext,
        ICheckInstanceOfTEntityClass<CategoryEntity> cateCheck,
        UserIdentifyService userIdentifyService
    ) : IRequestHandler<UpdateCategoryCommand, OkResponse>
    {
        public async Task<OkResponse> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            var dto = request.dto;
            if (string.IsNullOrEmpty(request.category_id) || string.IsNullOrWhiteSpace(request.category_id))
                throw new BadHttpRequestException("Category_id is required!");
            if (string.IsNullOrEmpty(dto.category_name) || string.IsNullOrWhiteSpace(dto.category_name))
                throw new BadHttpRequestException("Category_name is required!");
            var user_id = userIdentifyService.GetUserId();
            var currentUser = await userIdentifyService.FindCurrentUser(user_id);

            var currentCate = await dbContext.CategoryEntities
                .Where(x => x.category_id == request.category_id)
                .FirstOrDefaultAsync(cancellationToken);
            currentCate = cateCheck.CheckNullOrNot(currentCate, "Current category");
            currentUser.UpdateCategory(currentCate, dto.category_name);

            if (await unitOfWork.SaveChangeAsync(cancellationToken) > 0)
            {
                return new OkResponse($"User {currentUser.userName} updated category name successfully!");
            }
            throw new InvalidOperationException("Fail to save, nothing changes!");
        }
    }
}