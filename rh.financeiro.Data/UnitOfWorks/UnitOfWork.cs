using Microsoft.EntityFrameworkCore;
using rh.financeiro.Data.Repository;
using rh.financeiro.Domain.Interfaces.Repository;
using rh.financeiro.Domain.Interfaces.UnitOfWorks;

namespace ouroeprata.comprarapida.Data.UnitOfWorks;

public class UnitOfWork<TContext> : IUnitOfWork<TContext>, IDisposable where TContext : DbContext
{
    private readonly TContext _context;
    private bool disposed = false;

    public UnitOfWork(TContext context)
    {
        _context = context;
    }

    public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : class
    {
        return new GenericRepository<TEntity>(_context);
    }

    public void PersistChanges()
    {
        _context.SaveChanges();
        _context.ChangeTracker.Clear();
    }

    public async Task PersistChangesAsync(CancellationToken ct = default)
    {
        await _context.SaveChangesAsync(ct).ConfigureAwait(false);
        _context.ChangeTracker.Clear();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            {
                _context.Dispose();
            }
        }
        this.disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
