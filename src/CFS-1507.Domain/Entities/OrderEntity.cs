using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using CFS_1507.Domain.Common;

namespace CFS_1507.Domain.Entities
{
    public class OrderEntity : TEntityClass
    {
        [Key]
        public string order_id { get; set; } = null!;
        [ForeignKey(nameof(Cart))]
        public string? cart_id { get; set; }
        public CartEntity? Cart { get; set; }
        public string user_id { get; set; } = null!;
        public DateTimeOffset? created_at { get; set; }
        public DateTimeOffset? updated_at { get; set; }
        public virtual List<OrderItemsEntity> OrderItemsEntities { get; set; } = [];
        private OrderEntity() { }
        private OrderEntity(string cart_id, string user_id)
        {
            this.order_id = Guid.NewGuid().ToString();
            this.cart_id = cart_id;
            this.user_id = user_id;
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
            if (string.IsNullOrWhiteSpace(user_id))
            {
                throw new InvalidOperationException("User_id invalid");
            }
        }
        public static OrderEntity CreateOrder(string cart_id, string user_id)
        {
            return new OrderEntity(cart_id, user_id);
        }
        public OrderItemsEntity AddToOrderItems(string order_id, string product_id, int quantity)
        {
            return OrderItemsEntity.CreateOrderItem(order_id, product_id, quantity);
        }
    }
}