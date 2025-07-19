using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CFS_1507.Contract.Helper
{
    public class QueryHelperDto
    {
        public int pageSize { get; set; }
        public int pageNumber { get; set; }
        public bool sortByPrice { get; set; }
    }
}