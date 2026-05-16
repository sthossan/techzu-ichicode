using Microsoft.EntityFrameworkCore;
using Server.Domain.Entities;
using System.Linq.Expressions;
using Server.Application.Interfaces.Core;
using System.ComponentModel.DataAnnotations;
using Server.Shared.Exceptions;
using Server.Infrastructure.Persistence.Data;


namespace Server.Infrastructure.Persistence.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class//BaseEntity
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dbSet = _context.Set<T>();
        }

        public virtual async Task<T?> GetByIdAsync(Guid id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task<T> GetAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            // If you expect only one, use FirstOrDefaultAsync
            return (await _dbSet.AsNoTracking().FirstOrDefaultAsync(predicate, cancellationToken))!;
        }

        public virtual async Task<IReadOnlyList<T>> GetAllAsync()
        {
            return await _dbSet.AsNoTracking().ToListAsync();
        }

        public virtual async Task<IReadOnlyList<T>> GetAllAsync(params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet.AsNoTracking();

            if (includes != null && includes.Any())
            {
                query = includes.Aggregate(query, (current, includeExpression) => current.Include(includeExpression));
            }

            return await query.ToListAsync();
        }

        public virtual IQueryable<T> GetQueryable(params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet.AsNoTracking();

            if (includes != null && includes.Any())
            {
                query = includes.Aggregate(query, (current, includeExpression) => current.Include(includeExpression));
            }

            return query;
        }

        public virtual async Task<IReadOnlyList<T>> GetListAsync(
                        Expression<Func<T, bool>>? predicate = null,
                        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
                        List<Expression<Func<T, object>>>? includes = null,
                        bool disableTracking = true)
        {
            IQueryable<T> query = _dbSet;

            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            if (includes != null && includes.Any())
            {
                query = includes.Aggregate(query, (current, includeExpression) => current.Include(includeExpression));
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (orderBy != null)
            {
                return await orderBy(query).ToListAsync();
            }
            else
            {
                return await query.ToListAsync();
            }
        }

        public virtual async Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet.AsNoTracking();

            if (includes != null && includes.Any())
            {
                query = includes.Aggregate(query, (current, includeExpression) => current.Include(includeExpression));
            }

            return await query.FirstOrDefaultAsync(predicate);
        }

        public virtual async Task<T?> GetFirstOrDefaultForUpdateAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;

            if (includes != null && includes.Any())
            {
                query = includes.Aggregate(query, (current, includeExpression) => current.Include(includeExpression));
            }

            return await query.FirstOrDefaultAsync(predicate);
        }

        public virtual async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }

        public virtual async Task<bool> AnyAsync(Guid id)
        {
            return await _dbSet.AnyAsync(e => EF.Property<Guid>(e, "Id") == id);
        }

        public virtual async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            return entity;
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null)
        {
            if (predicate == null)
                return await _dbSet.CountAsync();
            return await _dbSet.CountAsync(predicate);
        }

        public virtual void Update(T entity)
        {
            _dbSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;

            // Audit field update needs to be conditional
            //if (entity is BaseEntity baseEntity) // Check if it's a BaseEntity
            //{
            //    baseEntity.SetUpdatedAt();
            //}
        }

        public virtual void Delete(T entity)
        {
            if (_context.Entry(entity).State == EntityState.Detached)
            {
                _dbSet.Attach(entity);
            }
            _dbSet.Remove(entity);
        }

        public virtual async Task DeleteAsync(T entity)
        {
            if (_context.Entry(entity).State == EntityState.Detached)
            {
                _dbSet.Attach(entity);
            }
            _dbSet.Remove(entity);
            await Task.CompletedTask;
        }

        public void ValidateModel(T model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model), "Model cannot be null.");

            var validationContext = new ValidationContext(model);
            var validationResults = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(model, validationContext, validationResults, validateAllProperties: true);

            if (!isValid)
            {
                var errors = validationResults.Select(r => r.ErrorMessage);
                throw new DomainValidationException(errors!, 400);
            }
        }

        public void ValidateDto<TDto>(TDto dto) where TDto : class
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto), "DTO cannot be null.");

            var validationContext = new ValidationContext(dto);
            var validationResults = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(dto, validationContext, validationResults, validateAllProperties: true);

            if (!isValid)
            {
                var errors = validationResults.Select(r => r.ErrorMessage);
                throw new DomainValidationException(errors!, 400);
            }
        }
    }
}