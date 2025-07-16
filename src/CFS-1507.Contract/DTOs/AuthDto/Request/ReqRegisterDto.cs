using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CFS_1507.Contract.DTOs.AuthDto.Request
{
    public class ReqRegisterDto
    {
        public string username { get; set; } = null!;
        public string password { get; set; } = null!;
        public string? email { get; set; }
    }
}