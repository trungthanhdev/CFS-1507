using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using CFS_1507.Domain.Common;

namespace CFS_1507.Domain.Entities
{
    public class OrderItemsEntity : TEntityClass
    {
        [Key]
        public string order_item_id { get; set; } = null!;
        [ForeignKey(nameof(Order))]
        public string order_id { get; set; } = null!;
        public OrderEntity? Order { get; set; }
        public string product_id { get; set; } = null!;
        public int quantity { get; set; }
        private OrderItemsEntity() { }
        private OrderItemsEntity(string order_id, string product_id, int quantity)
        {
            this.order_item_id = Guid.NewGuid().ToString();
            this.order_id = order_id;
            this.product_id = product_id;
            this.quantity = quantity;
            CheckValid();
        }
        private void CheckValid()
        {
            if (string.IsNullOrWhiteSpace(order_id))
            {
                throw new InvalidOperationException("Order_id invalid");
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
        public static OrderItemsEntity CreateOrderItem(string order_id, string product_id, int quantity)
        {
            return new OrderItemsEntity(order_id, product_id, quantity);
        }
    }
}