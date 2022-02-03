using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesWG.Shared.Requests
{
    public class PagedRequest
    {
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
        public string? SearchString { get; set; }
    }
}
