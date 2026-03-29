using System.Linq.Expressions;

namespace rh.financeiro.Domain.Interfaces.Repository;

public interface IGenericRepository<T> where T : class
{
    IQueryable<T> QueryableObject();
    Task<T?> GetAsync(Expression<Func<T, bool>> condition, params Expression<Func<T, object>>[] includes);
    Task<T?> GetByIdAsync(int id);
    Task<T?> GetByIdAsync(string id);
    Task<List<T>> GetAllAsync(Expression<Func<T, bool>> condition, params Expression<Func<T, object>>[] includes);
    Task<IEnumerable<T>> GetAllAsync();
    Task AddAsync(T entity);
    Task AddRangeAsync(List<T> entityList);
    Task UpdateAsync(T entity);
    void Delete(T entity);
    Task DeleteAllAsync();
    Task DeleteWhereAsync(Func<T, bool> condition);
    Task<int> CountAsync();
    Task Delete(IEnumerable<T> entities);
    Task UpdateRangeAsync<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;
}
