using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace CFS_1507.Contract.DTOs.ProductDto.Request
{
    public class ReqCreateProductDto
    {
        public string? product_name { get; set; }
        public double product_price { get; set; }
        public IFormFile? product_image { get; set; }
    }
}