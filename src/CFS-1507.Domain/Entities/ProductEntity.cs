using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CFS_1507.Domain.Common;

namespace CFS_1507.Domain.Entities
{
    public class ProductEntity : TEntityClass
    {
        [Key]
        public string product_id { get; set; } = null!;
        public string? product_name { get; set; }
        public double? product_price { get; set; }
    }
}