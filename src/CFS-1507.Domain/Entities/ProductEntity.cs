using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CFS_1507.Domain.Common;
using CFS_1507.Domain.Interfaces;

namespace CFS_1507.Domain.Entities
{
    public class ProductEntity : TEntityClass, IAggregrationRoot
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
        public virtual List<ProductCateEntity> ProductCateEntities { get; set; } = [];
        private ProductEntity() { }
        private ProductEntity(string product_name, double? product_price, string? product_description, string? product_image, int is_in_stock)
        {
            this.product_id = Guid.NewGuid().ToString();
            this.product_name = product_name;
            this.product_price = product_price;
            this.product_description = product_description;
            this.product_image = product_image;
            this.is_in_stock = is_in_stock;
            this.created_at = DateTimeOffset.UtcNow;
            this.updated_at = DateTimeOffset.UtcNow;
            CheckValid();
        }
        public void CheckValid()
        {
            if (string.IsNullOrWhiteSpace(product_name))
                throw new InvalidOperationException("Product name is required!");
            if (product_price < 0)
                throw new InvalidOperationException("Product price can not be negative");
            if (is_in_stock < 0)
                throw new InvalidOperationException("Stock can not be negative");
        }
        public static ProductEntity Create(string product_name, double? product_price, string? product_description, string? product_image, int is_in_stock, string category_id)
        {
            var newProduct = new ProductEntity(product_name, product_price, product_description, product_image, is_in_stock);
            newProduct.AddProductCategory(category_id);
            return newProduct;
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
        public void CheckQuantity(int quantity)
        {
            if (this.is_in_stock < quantity)
            {
                throw new InvalidOperationException("Quantity is greater than in stock!");
            }
        }
        public void UpdateIsInCart(int quantity)
        {
            CheckQuantity(quantity);
            is_in_cart += 1;
            this.updated_at = DateTimeOffset.UtcNow;
        }
        public void UndoIsInCart()
        {
            is_in_cart -= 1;
            if (is_in_cart < 0)
                is_in_cart = 0;
            this.updated_at = DateTimeOffset.UtcNow;
        }
        public void InCreaseIsBought(int quantity)
        {
            if (quantity < 0)
                throw new ArgumentException("Quantity must be greater than or equal to 0");
            if (is_in_stock < quantity)
                throw new ArgumentException($"In stock: {is_in_stock}");
            this.is_in_stock -= quantity;
            this.is_bought += quantity;
            this.updated_at = DateTimeOffset.UtcNow;
        }
        public ProductCateEntity AddProductCategory(string category_id)
        {
            var addCategory = ProductCateEntity.CreateProductCategory(this.product_id, category_id);
            ProductCateEntities.Add(addCategory);
            return addCategory;
        }
    }
}