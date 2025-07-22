using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using CFS_1507.Contract.DTOs.CartDto.Request;
using CFS_1507.Domain.Common;
using CFS_1507.Domain.DTOs;

namespace CFS_1507.Domain.Entities
{
    public class CartEntity : TEntityClass
    {
        [Key]
        public string cart_id { get; set; } = null!;
        public bool is_Paid { get; set; }
        [ForeignKey(nameof(User))]
        public string user_id { get; set; } = null!;
        public UserEntity? User { get; set; }
        public DateTimeOffset? created_at { get; set; }
        public DateTimeOffset? updated_at { get; set; }
        public OrderEntity? Order { get; set; }
        public virtual List<CartItemsEntity> CartItemsEntities { get; set; } = [];
        private CartEntity() { }
        private CartEntity(string user_id)
        {
            this.cart_id = Guid.NewGuid().ToString();
            this.user_id = user_id;
            this.is_Paid = false;
            this.created_at = DateTimeOffset.UtcNow;
            this.updated_at = DateTimeOffset.UtcNow;
        }
        public static CartEntity CreateCart(string user_id, List<ListCartItems> listCartItems)
        {
            var newCart = new CartEntity(user_id);
            foreach (var item in listCartItems)
            {
                if (string.IsNullOrWhiteSpace(item.product_id))
                    throw new InvalidOperationException("Product_id not found!");
                if (item.Product is null)
                    throw new InvalidOperationException("Product not found!");

                var newCartItem = CartItemsEntity.CreateCartItem(newCart.cart_id, item.quantity, item.product_id, item.Product);
                newCart.CartItemsEntities.Add(newCartItem);
            }
            return newCart;
        }
        public void CartPaid()
        {
            this.is_Paid = true;
        }

        public static CartItemsEntity AddItem(CartEntity cart, ListCartItems cartItems)
        {
            var existingItem = cart.CartItemsEntities
                .FirstOrDefault(i => i.product_id == cartItems.product_id);

            if (existingItem != null)
            {
                existingItem.UpdateCartItemQuantity(cartItems.quantity);
                return existingItem;
            }

            if (string.IsNullOrWhiteSpace(cartItems.product_id))
                throw new InvalidOperationException("Product_id not found!");
            if (cartItems.Product is null)
                throw new InvalidOperationException("Product not found!");

            var newCartItem = CartItemsEntity.CreateCartItem(cart.cart_id, cartItems.quantity, cartItems.product_id, cartItems.Product);
            cart.CartItemsEntities.Add(newCartItem);
            return newCartItem;
        }
        public void RemoveCartItemQuantity(CartItemsEntity cartItems, int quantity)
        {
            cartItems.RemoveCartItemQuantity(quantity);
        }
    }
}