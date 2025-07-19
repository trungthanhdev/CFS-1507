using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CFS_1507.Contract.DTOs.ProductDto.Request
{
    public class ReqUpdateProductDto
    {
        public string? product_name { get; set; }
        public double product_price { get; set; }
        public string? product_image { get; set; }
    }
}