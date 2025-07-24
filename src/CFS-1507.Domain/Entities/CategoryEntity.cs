using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CFS_1507.Domain.Common;

namespace CFS_1507.Domain.Entities
{
    public class CategoryEntity : TEntityClass
    {
        [Key]
        public string category_id { get; set; } = null!;
        public string category_name { get; set; } = null!;
        public DateTimeOffset? created_at { get; set; }
        public DateTimeOffset? updated_at { get; set; }
        public virtual List<ProductCateEntity>? ProductCateEntities { get; set; } = [];
        private CategoryEntity() { }
        private CategoryEntity(string category_name)
        {
            this.category_id = Guid.NewGuid().ToString();
            this.category_name = category_name;
            this.created_at = DateTimeOffset.UtcNow;
            this.updated_at = DateTimeOffset.UtcNow;

            CheckValid();
        }
        public void CheckValid()
        {
            if (string.IsNullOrWhiteSpace(category_name) || string.IsNullOrEmpty(category_name))
                throw new InvalidOperationException("Category name is required!");
        }
        public static CategoryEntity CreateCategory(string category_name)
        {
            return new CategoryEntity(category_name);
        }
        public void UpdateCategory(string newCateName)
        {
            this.category_name = newCateName ?? this.category_name;
            this.updated_at = DateTimeOffset.UtcNow;
        }
    }
}