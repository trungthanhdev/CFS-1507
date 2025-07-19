using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using CFS_1507.Domain.Common;

namespace CFS_1507.Domain.Entities
{
    public class AttachToEntity : TEntityClass
    {
        [Key]
        public string attach_to_id { get; set; } = null!;
        [ForeignKey(nameof(Role))]
        public string role_id { get; set; } = null!;
        public RoleEntity? Role { get; set; }
        [ForeignKey(nameof(User))]
        public string user_id { get; set; } = null!;
        public UserEntity? User { get; set; }
        public DateTimeOffset? created_at { get; set; }
        public DateTimeOffset? updated_at { get; set; }
        public AttachToEntity() { }
        private AttachToEntity(string role_id, string user_id)
        {
            this.attach_to_id = Guid.NewGuid().ToString();
            this.role_id = role_id;
            this.user_id = user_id;
            this.created_at = DateTimeOffset.UtcNow;
            this.updated_at = DateTimeOffset.UtcNow;
        }
        public static AttachToEntity Create(string role_id, string user_id)
        {
            return new AttachToEntity(role_id, user_id);
        }
        public void UpdateRole(string role_id)
        {
            this.role_id = role_id ?? this.role_id;
            this.updated_at = DateTimeOffset.UtcNow;
        }
    }
}