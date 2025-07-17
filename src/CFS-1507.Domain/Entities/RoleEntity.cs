using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CFS_1507.Domain.Common;

namespace CFS_1507.Domain.Entities
{
    public class RoleEntity : TEntityClass
    {
        [Key]
        public string role_id { get; set; } = null!;
        public string role_name { get; set; } = null!;
        public virtual List<AttachToEntity> AttachTos { get; set; } = [];
        public RoleEntity() { }
        private RoleEntity(string role_name)
        {
            this.role_id = Guid.NewGuid().ToString();
            this.role_name = role_name;
        }
    }
}