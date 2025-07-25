using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using CTCore.DynamicQuery.Common.Exceptions;
using Microsoft.AspNetCore.Http;

namespace CFS_1507.Infrastructure.Integrations
{
    public class MomoService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public MomoService(IConfiguration configuration)
        {
            _configuration = configuration;
            _httpClient = new HttpClient();
        }

        public async Task<string> CreatePaymentAsync(OrderInfoModel model)
        {
            var MomoApiUrl = _configuration["MomoAPI:MomoApiUrl"] ?? throw new NotFoundException("MomoApiUrl not found!");
            var PartnerCode = _configuration["MomoAPI:PartnerCode"] ?? throw new NotFoundException("PartnerCode not found!");
            var AccessKey = _configuration["MomoAPI:AccessKey"] ?? throw new NotFoundException("AccessKey not found!");
            var RequestType = _configuration["MomoAPI:RequestType"] ?? throw new NotFoundException("RequestType not found!");
            var RedirectUrl = _configuration["MomoAPI:RedirectUrl"] ?? throw new NotFoundException("RedirectUrl not found!");
            var NotifyUrl = _configuration["MomoAPI:NotifyUrl"] ?? throw new NotFoundException("NotifyUrl not found!");
            var SecretKey = _configuration["MomoAPI:SecretKey"] ?? throw new NotFoundException("SecretKey not found!");
            string extraData = "";

            var RequestId = Guid.NewGuid().ToString("N");
            string cartId = model.CartId?.Replace("-", "") ?? $"ORDER_{DateTime.Now.Ticks}";
            string rawData = $"accessKey={AccessKey}" +
                  $"&amount={model.Amount}" +
                  $"&extraData={extraData}" +
                  $"&ipnUrl={NotifyUrl}" +
                  $"&orderId={cartId}" +
                  $"&orderInfo={model.OrderInfo}" +
                  $"&partnerCode={PartnerCode}" +
                  $"&redirectUrl={RedirectUrl}" +
                  $"&requestId={RequestId}" +
                  $"&requestType={RequestType}";

            var signature = getSignature(rawData, SecretKey);

            var requestData = new
            {
                partnerCode = PartnerCode,
                accessKey = AccessKey,
                requestId = RequestId,
                amount = model.Amount.ToString(),
                orderId = cartId,
                orderInfo = model.OrderInfo,
                ipnUrl = NotifyUrl,
                extraData,
                redirectUrl = RedirectUrl,
                requestType = RequestType,
                signature,
                lang = "vi"
            };

            string json = JsonConvert.SerializeObject(requestData);
            // System.Console.WriteLine("Request data:");
            // Console.WriteLine(JsonConvert.SerializeObject(requestData, Formatting.Indented));

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(MomoApiUrl, content);
            // Console.WriteLine("Momo Raw Response:");
            // Console.WriteLine(response.Content);
            string responseBody = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Momo error: {responseBody}");
            }
            // Console.WriteLine($"Response: {responseBody}");

            if (string.IsNullOrEmpty(responseBody))
                throw new BadHttpRequestException("Response content is null or empty!");

            var momoResponse = JsonConvert.DeserializeObject<MomoResponseModel>(responseBody);
            if (momoResponse == null)
                throw new NotFoundException("Cannot parse data from MoMo response!");
            // System.Console.WriteLine($"Momo response: msg: {momoResponse.message}, payUrl: {momoResponse.payUrl},qrcode: {momoResponse.qrCodeUrl}");
            var payUrl = momoResponse.payUrl ?? "Can not get payUrl from Momo!";
            return payUrl;
        }

        private static String getSignature(String text, String key)
        {
            ASCIIEncoding encoding = new ASCIIEncoding();
            Byte[] textBytes = encoding.GetBytes(text);
            Byte[] keyBytes = encoding.GetBytes(key);
            Byte[] hashBytes;
            using (HMACSHA256 hash = new HMACSHA256(keyBytes))
                hashBytes = hash.ComputeHash(textBytes);
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }

    }

    public class OrderInfoModel
    {
        public string CartId { get; set; } = null!;
        public double Amount { get; set; }
        public string OrderInfo { get; set; } = null!;
    }

    public class MomoResponseModel
    {
        public long responseTime { get; set; }
        public string? message { get; set; }
        public int resultCode { get; set; }
        public string? payUrl { get; set; }
        public string? deeplink { get; set; }
        public string? qrCodeUrl { get; set; }
    }
}
