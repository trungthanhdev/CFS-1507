using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CFS_1507.Contract.DTOs.AuthDto.Response
{
    public class ResponseAuthDto
    {
        public string? access_token { get; set; }
        public string? refresh_token { get; set; }
        public string? user_name { get; set; }
        public string? user_id { get; set; }
        public string? role { get; set; }
    }
}