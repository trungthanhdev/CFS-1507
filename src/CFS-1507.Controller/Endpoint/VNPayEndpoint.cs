using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CFS_1507.Application.Usecases.MomoUC.Commands;
using CFS_1507.Application.Usecases.VNPayUC.Commands;
using CFS_1507.Contract.DTOs.VNPayDto.Request;
using CFS_1507.Domain.Interfaces;
using CFS_1507.Infrastructure.Helper;
using CFS_1507.Infrastructure.Hubs;
using CFS_1507.Infrastructure.Integrations.Payment;
using CTCore.DynamicQuery.Common.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Sprache;

namespace CFS_1507.Controller.Endpoint
{
    public class VNPayEndpoint
    {
        public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
        {
            var vn = endpoints
                .MapGroup("api/v1/vnpay")
                .WithDisplayName("VNPay");

            vn.MapPost("/vnpay-payment", PayByVNPay).RequireAuthorization();
            vn.MapMethods("/vnpay-handle-ipn", new[] { HttpMethods.Get, HttpMethods.Post }, HandleIPNVNPay);
            return endpoints;
        }
        public async Task<IResult> PayByVNPay(
          [FromBody] OrderInfo arg,
          [FromServices] IMediator mediator)
        {
            try
            {
                var result = await mediator.Send(new PayByVnPayCommand(arg));
                return Results.Redirect(result);
            }
            catch (BadHttpRequestException ex)
            {
                return Results.Problem(ex.Message, statusCode: 400);
            }
            catch (InvalidOperationException ex)
            {
                return Results.Problem(ex.Message, statusCode: 500);
            }
        }
        public async Task<IResult> HandleIPNVNPay(
          [FromServices] VnPayLibrary _vnPayLib,
          [FromServices] IConfiguration configuration,
          [FromServices] HttpContext httpContext,
          [FromServices] IRabbitMQPublisher publisher,
          [FromServices] IHubContext<MomoHub> momoHub,
          [FromServices] IMediator mediator)
        {
            try
            {
                // 1) Gom tham số từ Query + (nếu có) Form (VNPAY không gửi JSON)
                var data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                foreach (var kv in httpContext.Request.Query) data[kv.Key] = kv.Value.ToString();
                if (httpContext.Request.HasFormContentType)
                    foreach (var kv in httpContext.Request.Form) data[kv.Key] = kv.Value.ToString();

                // 2) Kiểm tra trường bắt buộc
                if (!data.TryGetValue("vnp_TxnRef", out var temp_cart_id) ||
                    !data.TryGetValue("vnp_ResponseCode", out var responseCode) ||
                    !data.TryGetValue("vnp_SecureHash", out var incomingHash))
                {
                    return Results.Json(new { RspCode = "99", Message = "Missing required fields" });
                }
                var secret = configuration["Vnpay:HashSecret"];
                var tmnCode = configuration["Vnpay:TmnCode"];
                if (string.IsNullOrWhiteSpace(secret) || string.IsNullOrWhiteSpace(tmnCode))
                    return Results.Json(new { RspCode = "99", Message = "Config missing" });

                if (!data.TryGetValue("vnp_TmnCode", out var incomingTmn) ||
                            !string.Equals(incomingTmn, tmnCode, StringComparison.Ordinal))
                {
                    return Results.Json(new { RspCode = "97", Message = "TmnCode mismatch" });
                }
                var sorted = new SortedDictionary<string, string>(StringComparer.Ordinal);
                foreach (var kv in data)
                {
                    if (kv.Key.Equals("vnp_SecureHash", StringComparison.OrdinalIgnoreCase) ||
                        kv.Key.Equals("vnp_SecureHashType", StringComparison.OrdinalIgnoreCase))
                        continue;
                    sorted[kv.Key] = kv.Value ?? "";
                }
                var raw = string.Join("&", sorted.Select(kv =>
                    $"{Uri.EscapeDataString(kv.Key)}={Uri.EscapeDataString(kv.Value)}"));


                using var hmac = new System.Security.Cryptography.HMACSHA512(Encoding.UTF8.GetBytes(secret));
                var computed = BitConverter.ToString(hmac.ComputeHash(Encoding.UTF8.GetBytes(raw)))
                                .Replace("-", "").ToLowerInvariant();

                if (!computed.Equals(incomingHash, StringComparison.OrdinalIgnoreCase))
                    return Results.Json(new { RspCode = "97", Message = "Invalid signature" });

                long.TryParse(data.GetValueOrDefault("vnp_Amount", "0"), out var amountX100);
                var amount = amountX100 / 100;
                var transactionNo = data.GetValueOrDefault("vnp_TransactionNo") ?? "";
                var orderInfo = Uri.UnescapeDataString(data.GetValueOrDefault("vnp_OrderInfo") ?? "");

                if (responseCode == "00")
                {
                    await publisher.Handle(temp_cart_id);
                    return Results.Json(new { RspCode = "00", Message = "Confirm Success" });
                }
                else
                {
                    var result = await mediator.Send(new RejectMomoPaymentCommand(temp_cart_id));
                    await momoHub.Clients.Group(result.user_cart_id)
                        .SendAsync("UserCancelOrder", result.user_cart_id, "UserCancelOrder");
                    return Results.Json(new { RspCode = "00", Message = "Confirm Failed" });
                }
            }
            catch (NotFoundException ex)
            {
                return Results.Problem(ex.Message, statusCode: 404);
            }
            catch (InvalidOperationException ex)
            {
                return Results.Problem(ex.Message, statusCode: 500);
            }
        }
    }
}