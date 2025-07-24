using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CFS_1507.Contract.DTOs.CateDto.Response;
using CFS_1507.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CFS_1507.Application.Usecases.CategoryUC.Queries
{
    public class GetAllCategoryQuery : IRequest<List<ResCategoryDto>>
    {
    }
    public class GetAllCategoryQueryHandler(
        AppDbContext dbContext
    ) : IRequestHandler<GetAllCategoryQuery, List<ResCategoryDto>>
    {
        public async Task<List<ResCategoryDto>> Handle(GetAllCategoryQuery request, CancellationToken cancellationToken)
        {
            var allCates = await dbContext.CategoryEntities
            .Select(x => new ResCategoryDto
            {
                category_id = x.category_id,
                category_name = x.category_name
            }).ToListAsync(cancellationToken);
            return allCates;
        }
    }
}