using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using CFS_1507.Domain.Common;

namespace CFS_1507.Domain.Entities
{
    public class TranslateEntity : TEntityClass
    {
        [Key]
        public string translate_id { get; set; } = null!;
        [ForeignKey(nameof(Product))]
        public string product_id { get; set; } = null!;
        public ProductEntity? Product { get; set; }
        public string? translate_name { get; set; }
        public string? translate_description { get; set; }
        public double? translate_price { get; set; }
        public bool is_deleted { get; private set; } = false;
        public string? translate_image { get; set; }
        public DateTimeOffset? created_at { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? updated_at { get; set; } = DateTimeOffset.UtcNow;
        private TranslateEntity() { }
        private TranslateEntity(string product_id, string? translate_name, double? translate_price, string? translate_description, string? translate_image)
        {
            this.translate_id = Guid.NewGuid().ToString();
            this.product_id = product_id;
            this.translate_name = translate_name;
            this.translate_price = translate_price;
            this.translate_description = translate_description;
            this.translate_image = translate_image;
            this.created_at = DateTimeOffset.Now;
            this.updated_at = DateTimeOffset.Now;
        }
        public static TranslateEntity Create(string product_id, string? translate_name, double? translate_price, string? translate_description, string? translate_image)
        {
            return new TranslateEntity(product_id, translate_name, translate_price, translate_description, translate_image);
        }
        public void Update(string? translate_name, double? translate_price, string? translate_description, string? translate_image)
        {
            this.translate_name = translate_name ?? this.translate_name;
            this.translate_price = translate_price ?? this.translate_price;
            this.translate_description = translate_description ?? this.translate_description;
            this.translate_image = translate_image ?? this.translate_image;
            this.updated_at = DateTimeOffset.UtcNow;
        }
    }
}