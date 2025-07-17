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
        public string? product_image { get; set; }
        private ProductEntity() { }
        private ProductEntity(string product_name, double product_price, string product_image)
        {
            this.product_id = Guid.NewGuid().ToString();
            this.product_name = product_image;
            this.product_price = product_price;
            this.product_image = product_image;
        }
        public static ProductEntity Create(string product_name, double product_price, string product_image)
        {
            return new ProductEntity(product_name, product_price, product_image);
        }
    }
}