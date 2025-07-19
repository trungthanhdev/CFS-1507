using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CFS_1507.Contract.DTOs.ProductDto.Request;
using CFS_1507.Domain.Entities;
using CFS_1507.Domain.Interfaces;
using CFS_1507.Infrastructure.Persistence;
using CTCore.DynamicQuery.Core.Domain.Interfaces;
using CTCore.DynamicQuery.Core.Primitives;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace CFS_1507.Application.Usecases.ProductUC.Commands
{
    public class CreateProductCommand(ReqCreateProductDto arg) : IRequest<OkResponse>
    {
        public ReqCreateProductDto Arg = arg;
    }
    public class CreateProductCommandHandler(
        IUnitOfWork unitOfWork,
        IRepositoryDefinition<ProductEntity> productRepo,
        ILocalStorage localStorage
    ) : IRequestHandler<CreateProductCommand, OkResponse>
    {
        public async Task<OkResponse> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            string image = "";
            var dto = request.Arg;
            if (string.IsNullOrWhiteSpace(dto.product_name))
                throw new BadHttpRequestException("Product name is required!");
            //1: validate imageFile
            if (dto.product_image != null || dto.product_image?.Length > 0)
            {
                string uploadImage = await localStorage.SaveImageAsync(dto.product_image, "image");
                image = uploadImage;
            }

            var newProduct = ProductEntity.Create(dto.product_name, dto.product_price, image);
            productRepo.Add(new List<ProductEntity> { newProduct });

            if (await unitOfWork.SaveChangeAsync(cancellationToken) > 0)
            {
                return new OkResponse("Create product successfully!");
            }
            throw new InvalidOperationException("Fail to save!");
        }
    }
}