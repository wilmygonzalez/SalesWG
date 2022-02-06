using System.Linq.Expressions;
using SalesWG.Server.Data;

namespace SalesWG.Server.Repositories
{
    public interface IAppRepository<T> where T : BaseEntity
    {
        IQueryable<T> GetAll();
        Task<T> GetByIdAsync(long id);
        IQueryable<T> FindAsync(Expression<Func<T, bool>> expression);
        Task InsertAsync(T entity);
        Task InsertRangeAsync(IEnumerable<T> entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task DeleteRangeAsync(IEnumerable<T> entity);
        Task<bool> Exists(long id);
    }
}
