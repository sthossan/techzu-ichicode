using Server.Application.Interfaces;
using Server.Application.Interfaces.Core;
using Server.Domain.Entities;
using Server.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Collections; // For Hashtable

namespace Server.Infrastructure.Persistence.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _dbContext;
        private Hashtable _repositories;
        private IDbContextTransaction? _transaction;
        private bool _disposed;

        public UnitOfWork(AppDbContext context)
        {
            _dbContext = context ?? throw new ArgumentNullException(nameof(context));
            _repositories = new Hashtable();
        }

        public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : class//BaseEntity
        {
            var type = typeof(TEntity);
            var key = $"Generic_{type.Name}";
            if (!_repositories.ContainsKey(key))
            {
                _repositories[key] = new GenericRepository<TEntity>(_dbContext);
            }

            return (IGenericRepository<TEntity>)_repositories[key]!;
        }

        public IBaseEntityRepository<TEntity> BaseRepository<TEntity>() where TEntity : BaseEntity
        {
            var type = typeof(TEntity);
            var key = $"Base_{type.Name}";
            if (!_repositories.ContainsKey(key))
            {
                _repositories[key] = new BaseEntityRepository<TEntity>(_dbContext);
            }

            return (IBaseEntityRepository<TEntity>)_repositories[key]!;
        }
        
        public async Task<int> SaveChangesAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _dbContext.Database.BeginTransactionAsync();
        }

        public async Task CommitAsync()
        {
            try
            {
                if (_transaction != null)
                {
                    await _transaction.CommitAsync();
                }
            }
            finally
            {
                if (_transaction != null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }

        public async Task RollbackAsync()
        {
            try
            {
                if (_transaction != null)
                {
                    await _transaction.RollbackAsync();
                }
            }
            finally
            {
                if (_transaction != null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }

        public void Dispose()
        {
            //_dbContext.Dispose();
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _transaction?.Dispose();
                    _dbContext.Dispose();
                }

                _disposed = true;
            }
        }
    }
}