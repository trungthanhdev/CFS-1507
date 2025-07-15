using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CFS_1507.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CFS_1507.Infrastructure.SeedData
{
    public class LanguageEntityConfiguration : IEntityTypeConfiguration<LanguageEntity>
    {
        public void Configure(EntityTypeBuilder<LanguageEntity> builder)
        {
            var languageList = new List<LanguageEntity>
            {
            new LanguageEntity
                {
                    language_id = "0196cce9-cadf-704d-bdcf-9edec1fc115d",
                    language_name = "Tiếng Việt",
                    language_code = "vi-VN"
                },
           new LanguageEntity
                {
                    language_id = "0196cce9-cadf-7881-894d-b4a5ac0889ff",
                    language_name = "English",
                    language_code = "en-US"
                }
            };
            builder.HasData(languageList);
        }
    }
}