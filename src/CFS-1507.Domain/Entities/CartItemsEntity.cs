using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using CFS_1507.Domain.Common;

namespace CFS_1507.Domain.Entities
{
    public class CartItemsEntity : TEntityClass
    {
        [Key]
        public string cart_item_id { get; set; } = null!;
        [ForeignKey(nameof(Cart))]
        public string cart_id { get; set; } = null!;
        public string status { get; set; } = null!;
        public CartEntity? Cart { get; set; }
        public int quantity { get; set; }
        public double product_price { get; set; }
        [ForeignKey(nameof(Product))]
        public string product_id { get; set; } = null!;
        public ProductEntity? Product { get; set; }
        public DateTimeOffset? created_at { get; set; }
        public DateTimeOffset? updated_at { get; set; }
        private CartItemsEntity() { }
        private CartItemsEntity(string cart_id, int quantity, string product_id, double product_price)
        {
            this.cart_item_id = Guid.NewGuid().ToString();
            this.cart_id = cart_id;
            this.quantity = quantity;
            this.product_id = product_id;
            this.product_price = product_price;
            this.status = "PENDING";
            this.created_at = DateTimeOffset.UtcNow;
            this.updated_at = DateTimeOffset.UtcNow;
            CheckValid();
        }
        private void CheckValid()
        {
            if (string.IsNullOrWhiteSpace(cart_id))
            {
                throw new InvalidOperationException("Cart_id invalid");
            }
            if (string.IsNullOrWhiteSpace(product_id))
            {
                throw new InvalidOperationException("Product_id invalid");
            }
            if (quantity <= 0)
            {
                throw new InvalidOperationException("Quantiy must be greater than 0");
            }
        }
        public void ChangeStatusToInProcess()
        {
            this.status = "IN_PROCESS";
            this.updated_at = DateTimeOffset.UtcNow;
        }
        public void ChangeStatusToCompleted()
        {
            this.status = "COMPLETED";
            this.updated_at = DateTimeOffset.UtcNow;
        }
        public static CartItemsEntity CreateCartItem(string cart_id, int itemQuantiy, string product_id, double product_price, ProductEntity product)
        {
            product.UpdateIsInCart(itemQuantiy);
            return new CartItemsEntity(cart_id, itemQuantiy, product_id, product_price) { Product = product };
        }
        public void UpdateCartItemQuantity(int itemQuantiy)
        {
            quantity += itemQuantiy;
            this.updated_at = DateTimeOffset.UtcNow;
        }
        public void RemoveCartItemQuantity(int itemQuantiy)
        {
            if (quantity < itemQuantiy)
                throw new InvalidOperationException("Invalid quantity remove");

            quantity -= itemQuantiy;

            if (quantity == 0)
                Product?.UndoIsInCart();
            this.updated_at = DateTimeOffset.UtcNow;
        }
    }
}