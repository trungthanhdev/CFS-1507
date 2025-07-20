using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using CFS_1507.Domain.Common;

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
        public static CartEntity CreateCart(string user_id)
        {
            return new CartEntity(user_id);
        }
        public void CartPaid()
        {
            this.is_Paid = true;
        }
    }
}