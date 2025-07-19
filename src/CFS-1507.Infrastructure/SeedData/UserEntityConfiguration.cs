using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CFS_1507.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Configuration;

namespace CFS_1507.Infrastructure.SeedData
{
    public class UserEntityConfiguration : IEntityTypeConfiguration<UserEntity>
    {
        public void Configure(EntityTypeBuilder<UserEntity> builder)
        {
            var createdDate = new DateTimeOffset(2024, 01, 01, 0, 0, 0, TimeSpan.Zero);
            var userList = new List<UserEntity>
            {
                new UserEntity
                {
                    user_id = "62b7cf0b-53c4-4e6d-b7e3-9c4fddb8f7da",
                    userName = "root",
                    email = "admin@example.com",
                    hashPassWord = "$2a$11$KNZLsWhag2eHt2FvvO/Zp.BfDDarMVYA8xMRlJmCt9iHREew38wme",
                    created_at = createdDate,
                    updated_at = createdDate
                },
                new UserEntity
                {
                    user_id = "8dbdf2f7-139b-4037-9f75-4f489313cb12",
                    userName = "dev",
                    email = "user1@example.com",
                    hashPassWord = "$2a$11$KNZLsWhag2eHt2FvvO/Zp.BfDDarMVYA8xMRlJmCt9iHREew38wme",
                    created_at = createdDate,
                    updated_at = createdDate
                }
            };
            builder.HasData(userList);
        }
    }
    public class RoleEntityConfiguration : IEntityTypeConfiguration<RoleEntity>
    {
        public void Configure(EntityTypeBuilder<RoleEntity> builder)
        {
            var roleList = new List<RoleEntity>
            {
                new RoleEntity
                {
                    role_id = "a47a25b5-6ef4-47b4-b942-52c2525a9a56",
                    role_name = "ADMIN"
                },
                new RoleEntity
                {
                    role_id = "f8e7280b-37c3-41d1-9a2d-6a1f40b25cd3",
                    role_name = "USER"
                }
            };
            builder.HasData(roleList);
        }
    }

    public class AttachTOEntityConfiguration : IEntityTypeConfiguration<AttachToEntity>
    {
        public void Configure(EntityTypeBuilder<AttachToEntity> builder)
        {
            var createdDate = new DateTimeOffset(2024, 01, 01, 0, 0, 0, TimeSpan.Zero);
            var attachToList = new List<AttachToEntity>
            {
                new AttachToEntity
                {
                    attach_to_id = "6e4f964b-8c79-4c7f-8db7-5c9df6b3a131",
                    role_id = "a47a25b5-6ef4-47b4-b942-52c2525a9a56",
                    user_id = "62b7cf0b-53c4-4e6d-b7e3-9c4fddb8f7da",
                    created_at = createdDate,
                    updated_at = createdDate
                },
                new AttachToEntity
                {
                    attach_to_id = "09fc9342-3bc3-4a01-81d9-2c38e6b6f5c4",
                    role_id = "f8e7280b-37c3-41d1-9a2d-6a1f40b25cd3",
                    user_id = "8dbdf2f7-139b-4037-9f75-4f489313cb12",
                    created_at = createdDate,
                    updated_at = createdDate
                },
            };
            builder.HasData(attachToList);
        }
    }
}