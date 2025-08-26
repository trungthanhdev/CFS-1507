using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CFS_1507.Contract.DTOs.VNPayDto.Request
{
    public class VNPayIPNDto
    {
        public long Amount { get; set; }
        public string ResponseCode { get; set; } = default!;
        public string TmnCode { get; set; } = default!;
        public string OrderId { get; set; } = default!;
        public string TransactionNo { get; set; } = default!;
        public string OrderInfo { get; set; } = default!;
        public DateTime PayDate { get; set; }
        public string SecureHashType { get; set; } = default!;
        public string SecureHash { get; set; } = default!;
    }
}