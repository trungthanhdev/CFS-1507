using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CFS_1507.Contract.DTOs.CateDto.Request
{
    public class ReqCreateCategoryDto
    {
        public string category_name { get; set; } = null!;
    }
    public class ReqUpdateCategoryDto
    {
        public string category_name { get; set; } = null!;
        // public string category_id { get; set; } = null!;
    }
}