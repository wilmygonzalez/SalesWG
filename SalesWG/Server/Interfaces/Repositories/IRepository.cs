using System.Linq.Expressions;
using SalesWG.Shared.Models;

namespace SalesWG.Server.Interfaces.Repositories
{
    public interface IRepository<T> where T : BaseEntity
    {
        IQueryable<T> GetAll();
        Task<T> GetByIdAsync(long id);
        IEnumerable<T> FindAsync(Expression<Func<T, bool>> expression);
        Task InsertAsync(T entity);
        Task InsertRangeAsync(IEnumerable<T> entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task DeleteRangeAsync(IEnumerable<T> entity);
        Task<bool> Exists(long id);
    }
}
