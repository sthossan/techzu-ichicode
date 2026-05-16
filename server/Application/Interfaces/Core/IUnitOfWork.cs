using Server.Domain.Entities;
using Server.Application.Interfaces;

namespace Server.Application.Interfaces.Core;

public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Get repository for entity type
    /// </summary>
    IGenericRepository<TEntity> Repository<TEntity>() where TEntity : class;
    IBaseEntityRepository<TEntity> BaseRepository<TEntity>() where TEntity : BaseEntity;

     
    /// <summary>
    /// Begin a new transaction
    /// </summary>
    Task BeginTransactionAsync();

    /// <summary>
    /// Commit the transaction
    /// </summary>
    Task CommitAsync();

    /// <summary>
    /// Rollback the transaction
    /// </summary>
    Task RollbackAsync();

    /// <summary>
    /// Save all changes made in this context to the database
    /// </summary>
    Task<int> SaveChangesAsync();
}
