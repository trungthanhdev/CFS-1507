using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CFS_1507.Contract.DTOs.ProductDto.Request;
using CFS_1507.Domain.Entities;
using CFS_1507.Domain.Interfaces;
using CFS_1507.Infrastructure.Persistence;
using CTCore.DynamicQuery.Common.Exceptions;
using CTCore.DynamicQuery.Core.Domain.Interfaces;
using CTCore.DynamicQuery.Core.Primitives;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CFS_1507.Application.Usecases.ProductUC.Commands
{
    public class CreateProductCommand(ReqCreateProductDto arg) : IRequest<OkResponse>
    {
        public ReqCreateProductDto Arg = arg;
    }
    public class CreateProductCommandHandler(
        IUnitOfWork unitOfWork,
        IRepositoryDefinition<ProductEntity> productRepo,
        IRepositoryDefinition<TranslateEntity> translateRepo,
        ILocalStorage localStorage,
        AppDbContext dbContext
    ) : IRequestHandler<CreateProductCommand, OkResponse>
    {
        public async Task<OkResponse> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            string image = "";
            var dto = request.Arg;
            if (string.IsNullOrWhiteSpace(dto.product_name) || string.IsNullOrEmpty(dto.product_name))
                throw new BadHttpRequestException("Product name is required!");
            if (string.IsNullOrWhiteSpace(dto.category_id) || string.IsNullOrEmpty(dto.category_id))
                throw new BadHttpRequestException("Category_id is required!");
            if (dto.product_price <= 0)
                throw new BadHttpRequestException("Product price must be greater than 0!");
            if (dto.is_in_stock <= 0)
                throw new BadHttpRequestException("Quantity in stock must be greater than 0!");

            if (dto.product_image != null || dto.product_image?.Length > 0)
            {
                string uploadImage = await localStorage.SaveImageAsync(dto.product_image, "image");
                image = uploadImage;
            }
            if (string.IsNullOrWhiteSpace(dto.translate_name))
            {
                dto.translate_name = dto.product_name + " default";
            }
            if (string.IsNullOrWhiteSpace(dto.translate_description) && dto.product_description != null)
            {
                dto.translate_description = dto.product_description + "default";
            }
            var currentCate = await dbContext.CategoryEntities
                .Where(x => x.category_id == dto.category_id)
                .FirstOrDefaultAsync(cancellationToken);
            if (currentCate is null) throw new NotFoundException("Category not found!");

            var newProduct = ProductEntity.Create(
                dto.product_name,
                dto.product_price,
                dto.product_description,
                image,
                dto.is_in_stock,
                currentCate.category_id
            );
            var newProductTranslate = TranslateEntity.Create(
                newProduct.product_id,
                dto.translate_name,
                dto.product_price,
                dto.translate_description,
                image
            );
            productRepo.Add(new List<ProductEntity> { newProduct });
            translateRepo.Add(new List<TranslateEntity> { newProductTranslate });

            if (await unitOfWork.SaveChangeAsync(cancellationToken) > 0)
            {
                return new OkResponse("Create product successfully!");
            }
            throw new InvalidOperationException("Fail to save!");
        }
    }
}