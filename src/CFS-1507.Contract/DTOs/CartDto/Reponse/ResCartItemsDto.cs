using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CFS_1507.Contract.DTOs.CartDto.Reponse
{
    public class ResCartItemsDto
    {
        public string? cart_item_id { get; set; }
        public int quantity { get; set; }
        public string? product_name { get; set; }
    }
    public class ResCartDto
    {
        public string? cart_id { get; set; }
        public List<ResCartItemsDto>? cartItems { get; set; }
    }
}