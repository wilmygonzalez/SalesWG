using Microsoft.EntityFrameworkCore;
using SalesWG.Shared.Helpers;
using SalesWG.Shared.Models;

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
            data.AddRange(await source.Skip(pageIndex * pageSize).Take(pageSize).ToListAsync());

            return new PagedList<T>(data, pageIndex, pageSize, count);
        }

        public static PagedResponse<T> ToPagedResponse<T>(this IPagedList<T> source)
        {
            return new PagedResponse<T>
            {
                PageIndex = source.PageIndex,
                PageSize = source.PageSize,
                TotalCount = source.TotalCount,
                TotalPages = source.TotalPages,
                HasPreviousPage = source.HasPreviousPage,
                HasNextPage = source.HasNextPage,
                Data = source.ToList()
            };
        }
    }
}
