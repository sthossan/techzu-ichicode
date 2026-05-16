using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Server.Application.Interfaces.Core
{
    public interface IGenericRepository<T> where T : class //BaseEntity
    {
        Task<T?> GetByIdAsync(Guid id);
        
        Task<T> GetAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<T>> GetAllAsync();
        Task<IReadOnlyList<T>> GetAllAsync(params Expression<Func<T, object>>[] includes);
        IQueryable<T> GetQueryable(params Expression<Func<T, object>>[] includes);
        Task<IReadOnlyList<T>> GetListAsync(Expression<Func<T, bool>>? predicate = null,
                                        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
                                        List<Expression<Func<T, object>>>? includes = null,
                                        bool disableTracking = true);
        Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);
        
        /// <summary>
        /// Gets first or default entity for update (tracked)
        /// </summary>
        Task<T?> GetFirstOrDefaultForUpdateAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);

        /// <summary>
        /// bool exists = await _repository.AnyAsync(x => x.Name == "nmae");
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// bool exists = await _repository.AnyAsync(goalId);
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> AnyAsync(Guid id);

        Task<T> AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
        Task DeleteAsync(T entity);
        Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);
        void ValidateModel(T model);
        void ValidateDto<TDto>(TDto dto) where TDto : class;
    }
}