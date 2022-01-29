using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesWG.Shared.Helpers
{
    public interface IPagedList<T> : IList<T>
    {
        public int PageIndex { get; }
        public int PageSize { get;}
        public int TotalCount { get; }
        public int TotalPages { get; }
        public bool HasPreviousPage { get; }
        public bool HasNextPage { get; }
    }
}
