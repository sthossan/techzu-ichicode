using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Identity.Client;
using Server.Domain.Entities;
using Server.Domain.Entities.Authorization;
using Server.Domain.Entities.Identity;
using System.Reflection;
using System.Security.Claims;
using System.Text.Json;

namespace Server.Infrastructure.Persistence.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AppDbContext(DbContextOptions<AppDbContext> options, IHttpContextAccessor httpContextAccessor) : base(options) { _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor)); }
        public DbSet<TestEntity> Tests { get; set; }
        public virtual DbSet<RefreshToken> RefreshTokens { get; set; }
        public virtual DbSet<SponsorshipRequest> SponsorshipRequests { get; set; }
        public virtual DbSet<WorkflowHistory> WorkflowHistories { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.Name == "Id") property.IsPrimaryKey();

                    if (property.ClrType == typeof(string))
                    {
                        if (!property.GetMaxLength().HasValue) property.SetMaxLength(255);

                        if (!property.IsNullable) property.IsNullable = false;
                    }

                    if (property.ClrType.IsEnum || Nullable.GetUnderlyingType(property.ClrType)?.IsEnum == true)
                    {
                        builder.Entity(entityType.ClrType).Property(property.Name).HasConversion<string>().HasMaxLength(40);
                    }

                    if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                    {
                        property.SetColumnType("timestamp with time zone");

                        if (property.Name == "CreatedAt") property.SetDefaultValueSql("NOW() AT TIME ZONE 'UTC'");
                    }

                    if (property.GetColumnType() == "text" && !IsSimpleType(property.ClrType))
                    {
                        var converter = new ValueConverter<object, string>(
                            v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null!),
                            v => JsonSerializer.Deserialize(v, property.ClrType, (JsonSerializerOptions)null!) ?? Activator.CreateInstance(property.ClrType)!
                        );

                        property.SetValueConverter(converter);
                    }
                }

                foreach (var foreignKey in entityType.GetForeignKeys())
                {
                    foreignKey.DeleteBehavior = DeleteBehavior.Cascade;
                }
            }


            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ProcessAuditFields();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void ProcessAuditFields()
        {
            var entries = ChangeTracker.Entries<BaseEntity>()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

            var currentUser = _httpContextAccessor?.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "system";

            foreach (var entityEntry in entries)
            {
                var now = DateTime.UtcNow;

                if (entityEntry.State == EntityState.Added)
                {
                    entityEntry.Entity.CreatedBy = currentUser;
                    entityEntry.Entity.CreatedAt = now;

                    entityEntry.Entity.UpdatedBy = currentUser;
                    entityEntry.Entity.UpdatedAt = now;
                }
                else if (entityEntry.State == EntityState.Modified)
                {
                    entityEntry.Property(x => x.CreatedBy).IsModified = false;
                    entityEntry.Property(x => x.CreatedAt).IsModified = false;

                    entityEntry.Entity.UpdatedBy = currentUser;
                    entityEntry.Entity.UpdatedAt = now;
                }
            }

        }

        private static bool IsSimpleType(Type type)
        {
            return type.IsPrimitive ||
                   type.IsValueType ||
                   type == typeof(string) ||
                   type == typeof(Guid) ||
                   type == typeof(DateTime) ||
                   type == typeof(decimal);
        }
    }
}
