using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Server.Infrastructure.Extensions
{
    public static class ModelBuilderExtensions
    {
        public static void HasData<TEntity>(this ModelBuilder modelBuilder, IEnumerable<TEntity> data) where TEntity : class
        {
            modelBuilder.Entity<TEntity>().HasData(data);
        }
    }
}
