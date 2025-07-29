using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CFS_1507.Domain.Entities;
using CFS_1507.Domain.Interfaces;
using CFS_1507.Infrastructure.Persistence;
using CTCore.DynamicQuery.Core.Domain.Interfaces;
using CTCore.DynamicQuery.Core.Primitives;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CFS_1507.Application.Usecases.MomoUC.Commands
{
    public record RejectMomoPaymentCommand(string temp_cart_id) : IRequest<RejectMomoResponse> { }
    public class RejectMomoPaymentCommandHandler(
        ICheckInstanceOfTEntityClass<CartEntity> cartCheck,
        IUnitOfWork unitOfWork,
        AppDbContext dbContext
    ) : IRequestHandler<RejectMomoPaymentCommand, RejectMomoResponse>
    {
        public async Task<RejectMomoResponse> Handle(RejectMomoPaymentCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.temp_cart_id))
                throw new BadHttpRequestException("Can not find temp_cart_id");
            var currentCart = await dbContext.CartEntities.Where(x => x.temp_cart_id == request.temp_cart_id).FirstOrDefaultAsync(cancellationToken);
            currentCart = cartCheck.CheckNullOrNot(currentCart, "Current cart");

            var cart_id = currentCart.cart_id;
            var cartItemsList = await dbContext.CartItemsEntities
                .Where(x => x.cart_id == cart_id && x.status == "IN_PROCESS")
                .ToListAsync(cancellationToken);
            foreach (var item in cartItemsList)
            {
                currentCart.ChangeStatusToPending(item);
            }
            currentCart.UnLockCart();
            var response = new RejectMomoResponse
            {
                msg = "User reject purchase successfully!",
                user_cart_id = currentCart.cart_id
            };
            if (await unitOfWork.SaveChangeAsync(cancellationToken) > 0)
            {
                return response;
            }
            await unitOfWork.RollbackAsync(cancellationToken);
            throw new InvalidOperationException("Fail to save, nothing changes!");
        }
    }
    public class RejectMomoResponse
    {
        public string msg { get; set; } = null!;
        public string user_cart_id { get; set; } = null!;
    }
}