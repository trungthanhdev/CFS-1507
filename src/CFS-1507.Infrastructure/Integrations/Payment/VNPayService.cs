using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CFS_1507.Infrastructure.Helper;
using CTCore.DynamicQuery.Common.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CFS_1507.Infrastructure.Integrations.Payment
{
    public class VNPayService
    {
        private readonly IConfiguration _configuration;
        private readonly VnPayLibrary _vnPayLib;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<VNPayService> _logger;
        public VNPayService(IConfiguration configuration, VnPayLibrary vnPayLibrary, IHttpContextAccessor httpContextAccessor, ILogger<VNPayService> logger)
        {
            _configuration = configuration;
            _vnPayLib = vnPayLibrary;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }
        public string CreateVNPay(OrderInfo model)
        {
            _logger.LogInformation($"vnp_ReturnUrl: {_configuration["Vnpay:VNPReturnUrl"]}");
            string vnp_ReturnUrl = _configuration["Vnpay:VNPReturnUrl"] ?? throw new NotFoundException("VNPReturnUrl not found!");
            string vnp_Url = _configuration["Vnpay:BaseUrl"] ?? throw new NotFoundException("vnp_Url not found!");
            string vnp_IPN = _configuration["Vnpay:IpnUrl"] ?? throw new NotFoundException("IpnUrl not found!");
            string vnp_TmnCode = _configuration["Vnpay:TmnCode"] ?? throw new NotFoundException("vnp_TmnCode not found!");
            string vnp_HashSecret = _configuration["Vnpay:HashSecret"] ?? throw new NotFoundException("vnp_HashSecret not found!");
            string vnp_Command = _configuration["Vnpay:Command"] ?? throw new NotFoundException("Command not found!");
            string vnp_Version = _configuration["Vnpay:Version"] ?? throw new NotFoundException("Version not found!");
            string vnp_BankCode = _configuration["Vnpay:BankCode"] ?? throw new NotFoundException("BankCode not found!");
            string vnp_CurrCode = _configuration["Vnpay:CurrCode"] ?? throw new NotFoundException("CurrCode not found!");
            string vnp_Locale = _configuration["Vnpay:Locale"] ?? throw new NotFoundException("Locale not found!");

            if (string.IsNullOrWhiteSpace(vnp_ReturnUrl)) throw new NotFoundException("VNPReturnUrl missing/empty");
            if (string.IsNullOrWhiteSpace(vnp_Url)) throw new NotFoundException("BaseUrl missing/empty");
            if (string.IsNullOrWhiteSpace(vnp_TmnCode)) throw new NotFoundException("TmnCode missing/empty");
            if (string.IsNullOrWhiteSpace(vnp_IPN)) throw new NotFoundException("vnp_IPN missing/empty");
            if (string.IsNullOrWhiteSpace(vnp_HashSecret)) throw new NotFoundException("HashSecret missing/empty");
            if (model is null) throw new ArgumentNullException(nameof(model));
            if (string.IsNullOrWhiteSpace(model.OrderId)) throw new BadHttpRequestException("OrderId is required");
            if (model.Amount <= 0) throw new BadHttpRequestException("Amount must be > 0");

            DateTime createdUtc = model.CreatedDate;
            TimeZoneInfo tzVN;
            try { tzVN = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"); } // Windows
            catch { tzVN = TimeZoneInfo.FindSystemTimeZoneById("Asia/Ho_Chi_Minh"); }    // Linux
            var createdVN = TimeZoneInfo.ConvertTimeFromUtc(createdUtc, tzVN);
            var expireVN = createdVN.AddMinutes(15);
            var ip = Utils.GetIpAddress(_httpContextAccessor);

            _vnPayLib.AddRequestData("vnp_Version", vnp_Version);
            _vnPayLib.AddRequestData("vnp_Command", vnp_Command);
            _vnPayLib.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            _vnPayLib.AddRequestData("vnp_BankCode", vnp_BankCode);
            _vnPayLib.AddRequestData("vnp_CurrCode", vnp_CurrCode);
            _vnPayLib.AddRequestData("vnp_Locale", vnp_Locale);
            _vnPayLib.AddRequestData("vnp_Amount", (model.Amount * 100).ToString());
            _vnPayLib.AddRequestData("vnp_CreateDate", createdVN.ToString("yyyyMMddHHmmss"));
            _vnPayLib.AddRequestData("vnp_ExpireDate", expireVN.ToString("yyyyMMddHHmmss"));
            _vnPayLib.AddRequestData("vnp_IpAddr", ip);
            _vnPayLib.AddRequestData("vnp_IpnUrl", vnp_IPN);
            _vnPayLib.AddRequestData("vnp_OrderInfo", "Thanh toan don hang:" + model.OrderId);
            _vnPayLib.AddRequestData("vnp_OrderType", "other");
            _vnPayLib.AddRequestData("vnp_ReturnUrl", vnp_ReturnUrl);
            _vnPayLib.AddRequestData("vnp_TxnRef", model.OrderId!);

            string paymentUrl = _vnPayLib.CreateRequestUrl(vnp_Url, vnp_HashSecret);
            _logger.LogInformation($"VNPAY URL: {paymentUrl}");
            return paymentUrl;
        }
    }
    public class OrderInfo
    {
        public string? OrderId { get; set; }
        public long Amount { get; set; }
        public string? OrderDesc { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}