using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using CFS_1507.Domain.Common;

namespace CFS_1507.Domain.Entities
{
    public class ProductCateEntity : TEntityClass
    {
        [Key]
        public string product_cate_id { get; set; } = null!;
        [ForeignKey(nameof(Product))]
        public string product_id { get; set; } = null!;
        public ProductEntity? Product { get; set; }
        [ForeignKey(nameof(Category))]
        public string category_id { get; set; } = null!;
        public CategoryEntity? Category { get; set; }
        public DateTimeOffset? created_at { get; set; }
        public DateTimeOffset? updated_at { get; set; }
        private ProductCateEntity() { }
        private ProductCateEntity(string category_id, string product_id)
        {
            this.product_cate_id = Guid.NewGuid().ToString();
            this.category_id = category_id;
            this.product_id = product_id;
            this.created_at = DateTimeOffset.UtcNow;
            this.updated_at = DateTimeOffset.UtcNow;

            CheckValid();
        }
        public void CheckValid()
        {
            if (string.IsNullOrWhiteSpace(product_id) || string.IsNullOrEmpty(product_id))
                throw new InvalidOperationException("Product_id is required!");
            if (string.IsNullOrWhiteSpace(category_id) || string.IsNullOrEmpty(category_id))
                throw new InvalidOperationException("Product_id is required!");
        }
        public static ProductCateEntity CreateProductCategory(string product_id, string category_id)
        {
            return new ProductCateEntity(category_id, product_id);
        }
        public void UpdateCate(string new_category_id)
        {
            if (!string.IsNullOrWhiteSpace(new_category_id))
            {
                this.category_id = new_category_id ?? this.category_id;
                this.updated_at = DateTimeOffset.UtcNow;
            }
        }
    }
}