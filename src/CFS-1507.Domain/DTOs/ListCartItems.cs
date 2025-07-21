using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CFS_1507.Domain.Entities;

namespace CFS_1507.Domain.DTOs
{
    public class ListCartItems
    {
        public string? product_id { get; set; }
        public string? product_name { get; set; }
        public int quantity { get; set; }
        public ProductEntity? Product { get; set; }
    }
}