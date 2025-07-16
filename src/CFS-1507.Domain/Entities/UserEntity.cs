using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CFS_1507.Contract.DTOs.AuthDto.Request;
using CFS_1507.Domain.Common;
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

        private UserEntity() { }
        private UserEntity(string userName, string? email, string hashPassWord)
        {
            this.user_id = Guid.NewGuid().ToString();
            this.userName = userName;
            this.email = email;
            this.hashPassWord = hashPassWord;
        }
        public static UserEntity Create(ReqRegisterDto arg)
        {
            var hashPassWord = BCrypt.Net.BCrypt.HashPassword(arg.password);
            return new UserEntity(arg.username, arg.email, hashPassWord);
        }
    }
}