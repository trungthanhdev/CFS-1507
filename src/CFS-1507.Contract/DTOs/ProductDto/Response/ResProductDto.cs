using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CFS_1507.Contract.DTOs.ProductDto.Response
{
    public class ResProductDto
    {
        public string? product_id { get; set; }
        public string? translate_id { get; set; }
        public string? product_name { get; set; }
        public double? product_price { get; set; }
        public string? product_image { get; set; }
        public string? product_description { get; set; }
    }
    public class ResponseDto
    {
        public List<ResProductPageDto>? data { get; set; }
    }
    public class ResProductPageDto
    {
        public string? category_name { get; set; }
        public List<ResProductItemDto>? listProductItems { get; set; }
    }
    public class ResProductItemDto
    {
        public string? product_id { get; set; }
        public string? product_name { get; set; }
        public double? product_price { get; set; }
        public string? product_description { get; set; }
    }
}