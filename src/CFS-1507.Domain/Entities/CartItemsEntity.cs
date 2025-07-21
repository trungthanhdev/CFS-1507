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
        public CartEntity? Cart { get; set; }
        public int quantity { get; set; }
        [ForeignKey(nameof(Product))]
        public string product_id { get; set; } = null!;
        public ProductEntity? Product { get; set; }
        private CartItemsEntity() { }
        private CartItemsEntity(string cart_id, int quantity, string product_id)
        {
            this.cart_item_id = Guid.NewGuid().ToString();
            this.cart_id = cart_id;
            this.quantity = quantity;
            this.product_id = product_id;
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
        public static CartItemsEntity CreateCartItem(string cart_id, int itemQuantiy, string product_id, ProductEntity product)
        {
            product.UpdateIsInCart(itemQuantiy);
            return new CartItemsEntity(cart_id, itemQuantiy, product_id);
        }
    }
}