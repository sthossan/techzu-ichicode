using Server.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Application.Interfaces.Core
{
    public interface IBaseEntityRepository<TEntity> : IGenericRepository<TEntity> where TEntity : BaseEntity
    {
    }
}
