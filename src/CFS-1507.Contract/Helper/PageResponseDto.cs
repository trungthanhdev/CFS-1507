using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CFS_1507.Contract.Helper
{
    public class PageResponseDto<TDto>
    {
        public int pageNumber { get; set; }
        public int pageSize { get; set; }
        public int total { get; set; }
        public bool? sortByPrice { get; set; } = false;
        public IEnumerable<TDto> data { get; set; } = [];
    }
}