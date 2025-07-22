using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CFS_1507.Contract.DTOs.CartDto.Request
{
    public class ReqAddToCartDto
    {
        public List<CartItemDto>? listCartItems { get; set; }
    }
    public class CartItemDto
    {
        public string product_id { get; set; } = null!;
        public int quantity { get; set; }
    }
    public class ReqRemoveQuantityDto
    {
        public List<CartItemDto>? listCartItems { get; set; }
    }
}