using Microsoft.EntityFrameworkCore;
using rh.financeiro.Domain.Interfaces.Repository;
using System.Linq.Expressions;

namespace rh.financeiro.Data.Repository;

public class GenericRepository<T> : IGenericRepository<T> where T : class

{
    protected readonly DbContext _context;
    private readonly DbSet<T> _dbSet;

    public GenericRepository(DbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public IQueryable<T> QueryableObject()
    {
        return _dbSet.AsQueryable();
    }

    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();

    }

    public async Task AddRangeAsync(List<T> entityList)
    {
        await _dbSet.AddRangeAsync(entityList);
        await _context.SaveChangesAsync();
    }

    public void Delete(T entity)
    {
        _dbSet.Remove(entity);
    }

    public async Task Delete(IEnumerable<T> entities)
    {
        _dbSet.RemoveRange(entities);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAllAsync()
    {
        var allEntities = await _dbSet.ToListAsync();
        _dbSet.RemoveRange(allEntities);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteWhereAsync(Func<T, bool> condition)
    {
        var entitiesToDelete = _dbSet.Where(condition).ToList();
        _dbSet.RemoveRange(entitiesToDelete);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task<T?> GetAsync(Expression<Func<T, bool>> condition, params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> dbQuery = _dbSet;

        foreach (var include in includes)
            dbQuery = dbQuery.Include(include);

        return await dbQuery.FirstOrDefaultAsync(condition)!;
    }

    public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>> condition, params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> dbQuery = _dbSet;

        foreach (var include in includes)
            dbQuery = dbQuery.Include(include);

        return await dbQuery.Where(condition).ToListAsync();
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<T?> GetByIdAsync(string id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task UpdateAsync(T entity)
    {
        _context.Entry(entity).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }
    public async Task UpdateRangeAsync<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
    {
        if (_context == null)
        {
            throw new InvalidOperationException("Context is not set.");
        }

        _context.Set<TEntity>().UpdateRange(entities);
        await _context.SaveChangesAsync();
    }

    public async Task<int> CountAsync()
    {
        return await _dbSet.CountAsync();
    }
}

