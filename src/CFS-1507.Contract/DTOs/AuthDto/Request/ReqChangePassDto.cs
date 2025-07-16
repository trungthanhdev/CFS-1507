using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CFS_1507.Contract.DTOs.AuthDto.Request
{
    public class ReqChangePassDto
    {
        public string old_pass { get; set; } = null!;
        public string new_pass { get; set; } = null!;
    }
}