using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using CFS_1507.Domain.Common;

namespace CFS_1507.Domain.Entities
{
    public class BlackListEntity : TEntityClass
    {
        [Key]
        public string blacklist_id { get; set; } = null!;
        [ForeignKey(nameof(User))]
        public string user_id { get; set; } = null!;
        public UserEntity? User { get; set; }
        public DateTimeOffset create_at { get; set; } = DateTimeOffset.UtcNow;
        public string token_id { get; set; } = null!;
        private BlackListEntity() { }
        private BlackListEntity(string user_id, string token_id)
        {
            this.blacklist_id = Guid.NewGuid().ToString();
            this.user_id = user_id;
            this.token_id = token_id;
        }
        public static BlackListEntity Create(string user_id, string token_id)
        {
            return new BlackListEntity(user_id, token_id);
        }
    }
}