using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesWG.Shared.Models
{
    public class PagedResponse<T>
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }
        public IList<T> Data { get; set; } = new List<T>();
    }
}
