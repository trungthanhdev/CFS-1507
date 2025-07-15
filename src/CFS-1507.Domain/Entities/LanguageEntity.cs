using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CFS_1507.Domain.Entities
{
    public class LanguageEntity
    {
        [Key]
        public string language_id { get; set; } = null!;
        public string language_name { get; set; } = null!;
        public string language_code { get; set; } = null!;
        public LanguageEntity() { }
        private LanguageEntity(string language_name, string language_code)
        {
            this.language_id = Guid.NewGuid().ToString();
            this.language_code = language_code;
            this.language_name = language_name;
        }

    }

}