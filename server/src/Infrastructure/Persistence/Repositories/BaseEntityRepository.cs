using Server.Application.Interfaces.Core;
using Server.Domain.Entities;
using Server.Infrastructure.Persistence.Data;
using Server.Infrastructure.Persistence.Repositories;

namespace Server.Infrastructure.Persistence.Repositories
{
    public class BaseEntityRepository<TEntity> : GenericRepository<TEntity>, IBaseEntityRepository<TEntity>
        where TEntity : BaseEntity
    {
        public BaseEntityRepository(AppDbContext context) : base(context) { }
    }
}
