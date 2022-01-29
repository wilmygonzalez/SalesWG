using Microsoft.EntityFrameworkCore;
using SalesWG.Shared.Helpers;
using SalesWG.Shared.Helpers.Interfaces;

namespace SalesWG.Server.Helpers
{
    public static class AppExtensions
    {
        public static async Task<IPagedList<T>> ToPagedListAsync<T>(this IQueryable<T> source, int pageIndex, int pageSize)
        {
            if (source == null)
                return new PagedList<T>(new List<T>(), pageIndex, pageSize);

            pageSize = Math.Max(pageSize, 1);

            var count = await source.CountAsync();

            var data = new List<T>();

            return new PagedList<T>(data, pageIndex, pageSize, count); 
        }
    }
}
