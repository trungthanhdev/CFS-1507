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
    public record CreateCategoryCommand(ReqCreateCategoryDto dto) : IRequest<OkResponse> { }
    public class CreateCategoryCommandHandler(
        IUnitOfWork unitOfWork,
        IRepositoryDefinition<CategoryEntity> cateRepo,
        AppDbContext dbContext,
        UserIdentifyService userIdentifyService
    ) : IRequestHandler<CreateCategoryCommand, OkResponse>
    {
        public async Task<OkResponse> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.dto.category_name) || string.IsNullOrEmpty(request.dto.category_name))
                throw new BadHttpRequestException("Category name is required!");

            var user_id = userIdentifyService.GetUserId();
            var currentUser = await userIdentifyService.FindCurrentUser(user_id);

            var existedCate = await dbContext.CategoryEntities
                .Where(x => x.category_name == request.dto.category_name)
                .FirstOrDefaultAsync(cancellationToken);
            if (existedCate != null)
                throw new BadHttpRequestException("Category existed!");

            var newCate = currentUser.CreateNewCategory(request.dto.category_name);
            cateRepo.Add(new List<CategoryEntity> { newCate });
            if (await unitOfWork.SaveChangeAsync(cancellationToken) > 0)
            {
                return new OkResponse($"User {currentUser.userName} created new category {newCate.category_name} successfully!");
            }
            throw new InvalidOperationException("Fail to save, nothing changes!");
        }
    }
}