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
        public string? translate_name { get; set; }
        public string? product_description { get; set; }
        public string? translate_description { get; set; }
        public double? product_price { get; set; }
        public int is_in_stock { get; set; }
        public string? category_id { get; set; }
        public IFormFile? product_image { get; set; }
    }
}