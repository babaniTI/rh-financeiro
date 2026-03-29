using rh.financeiro.Domain.Interfaces.Repository;

namespace rh.financeiro.Domain.Interfaces.UnitOfWorks;

public interface IUnitOfWork<TContext> where TContext : class
{
    IGenericRepository<TEntity> Repository<TEntity>() where TEntity : class;
    void Dispose();
    void PersistChanges();
    Task PersistChangesAsync(CancellationToken ct = default);
}
