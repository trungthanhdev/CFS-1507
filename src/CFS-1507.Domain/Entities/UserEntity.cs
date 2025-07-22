using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CFS_1507.Contract.DTOs.AuthDto.Request;
using CFS_1507.Contract.DTOs.CartDto.Request;
using CFS_1507.Contract.DTOs.ProductDto.Request;
using CFS_1507.Domain.Common;
using CFS_1507.Domain.DTOs;
using CFS_1507.Domain.Interfaces;

namespace CFS_1507.Domain.Entities
{
    public class UserEntity : TEntityClass, IAggregrationRoot
    {
        [Key]
        public string user_id { get; set; } = null!;
        public string userName { get; set; } = null!;
        public string? email { get; set; }
        public string hashPassWord { get; set; } = null!;
        public DateTimeOffset? created_at { get; set; }
        public DateTimeOffset? updated_at { get; set; }
        public virtual List<BlackListEntity> BlackListEntities { get; set; } = [];
        public virtual List<AttachToEntity> AttachToEntities { get; set; } = [];
        public virtual List<CartEntity> CartEntities { get; set; } = [];

        public UserEntity() { }
        private UserEntity(string userName, string? email, string hashPassWord)
        {
            this.user_id = Guid.NewGuid().ToString();
            this.userName = userName;
            this.email = email;
            this.hashPassWord = hashPassWord;
            this.created_at = DateTimeOffset.UtcNow;
            this.updated_at = DateTimeOffset.UtcNow;
        }
        public static UserEntity Create(ReqRegisterDto arg)
        {
            var hashPassWord = BCrypt.Net.BCrypt.HashPassword(arg.password);
            return new UserEntity(arg.username, arg.email, hashPassWord);
        }
        public bool CheckPassword(string rawPassWord)
        {
            return BCrypt.Net.BCrypt.Verify(rawPassWord, this.hashPassWord);
        }

        public void ChangePassword(string newPassword)
        {
            var newHashPassWord = BCrypt.Net.BCrypt.HashPassword(newPassword);
            this.hashPassWord = newHashPassWord;
            this.updated_at = DateTimeOffset.UtcNow;
        }

        public void AddRole(string role_id)
        {
            if (!AttachToEntities.Any(r => r.role_id == role_id))
            {
                var roleAttachTo = AttachToEntity.Create(role_id, this.user_id);
                AttachToEntities.Add(roleAttachTo);
            }
        }
        public void UpdateRole(AttachToEntity currentRole, RoleEntity newRole)
        {
            currentRole.UpdateRole(newRole.role_id);
        }
        public void UpdateProduct(ReqUpdateProductDto dto, ProductEntity product)
        {
            product.Update(dto.product_name, dto.product_image, dto.product_description, dto.product_price, dto.is_in_stock);
        }
        public void UpdateProductTranslate(ReqUpdateProductDto dto, TranslateEntity product)
        {
            product.Update(dto.translate_name, dto.product_price, dto.translate_description, dto.product_image);
        }
        public void ToggleDeleteProduct(ProductEntity product)
        {
            product.ToggleDeleteProduct();
        }

        public CartEntity AddToCart(List<ListCartItems> listCartItems)
        {
            var newCart = CartEntity.CreateCart(this.user_id, listCartItems);
            CartEntities.Add(newCart);
            return newCart;
        }
        public CartItemsEntity AddCartItem(CartEntity cart, ListCartItems cartItems)
        {
            var newCartItem = CartEntity.AddItem(cart, cartItems);
            return newCartItem;
        }
        public void RemoveCartItemQuantity(CartEntity cart, CartItemsEntity cartItems, int quantity)
        {
            cart.RemoveCartItemQuantity(cartItems, quantity);
        }
    }
}