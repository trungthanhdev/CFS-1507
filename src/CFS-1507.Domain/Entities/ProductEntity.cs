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
        public string? product_description { get; set; }
        public double? product_price { get; set; }
        public bool is_deleted { get; private set; } = false;
        public string? product_image { get; set; }
        public int is_in_stock { get; set; }
        public int is_bought { get; set; }
        public int is_in_cart { get; set; }
        public DateTimeOffset? created_at { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? updated_at { get; set; } = DateTimeOffset.UtcNow;
        public virtual List<TranslateEntity> TranslateEntities { get; set; } = [];
        public virtual List<CartItemsEntity> CartItemsEntities { get; set; } = [];
        private ProductEntity() { }
        private ProductEntity(string product_name, double? product_price, string? product_description, string? product_image, int is_in_stock)
        {
            this.product_id = Guid.NewGuid().ToString();
            this.product_name = product_name;
            this.product_price = product_price;
            this.product_description = product_description;
            this.product_image = product_image;
            this.is_in_stock = is_in_stock;
        }
        public static ProductEntity Create(string product_name, double? product_price, string? product_description, string? product_image, int is_in_stock)
        {
            return new ProductEntity(product_name, product_price, product_description, product_image, is_in_stock);
        }
        public void Update(string? product_name, string? product_image, string? product_description, double? product_price, int? is_in_stock)
        {
            this.product_name = product_name ?? this.product_name;
            this.product_image = product_image ?? this.product_image;
            this.product_price = product_price ?? this.product_price;
            this.product_description = product_description ?? this.product_description;
            this.is_in_stock = is_in_stock ?? this.is_in_stock;
            this.updated_at = DateTimeOffset.UtcNow;
        }
        public void ToggleDeleteProduct()
        {
            this.is_deleted = !is_deleted;
            this.updated_at = DateTimeOffset.UtcNow;
        }
    }
}