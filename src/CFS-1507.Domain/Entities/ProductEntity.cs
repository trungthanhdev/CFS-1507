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
        public bool is_deleted { get; private set; } = false;
        public string? product_image { get; set; }
        public DateTimeOffset? created_at { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? updated_at { get; set; } = DateTimeOffset.UtcNow;
        private ProductEntity() { }
        private ProductEntity(string product_name, double product_price, string product_image)
        {
            this.product_id = Guid.NewGuid().ToString();
            this.product_name = product_name;
            this.product_price = product_price;
            this.product_image = product_image;
        }
        public static ProductEntity Create(string product_name, double product_price, string product_image)
        {
            return new ProductEntity(product_name, product_price, product_image);
        }
        public void Update(string? product_name, string? product_image, double? product_price)
        {
            this.product_name = product_name ?? this.product_name;
            this.product_image = product_image ?? this.product_image;
            this.product_price = product_price ?? this.product_price;
            this.updated_at = DateTimeOffset.UtcNow;
        }

        public void ToggleDeleteProduct()
        {
            this.is_deleted = !is_deleted;
            this.updated_at = DateTimeOffset.UtcNow;
        }
    }
}