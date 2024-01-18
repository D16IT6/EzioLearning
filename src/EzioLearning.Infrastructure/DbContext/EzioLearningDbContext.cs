using EzioLearning.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using EzioLearning.Domain.Common;

namespace EzioLearning.Infrastructure.DbContext
{
    public class EzioLearningDbContext(DbContextOptions options) : IdentityDbContext<AppUser, AppRole, Guid>(options)
    {
        protected override void OnModelCreating(ModelBuilder builder)
        {

            base.OnModelCreating(builder);

            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                var tableName = entityType.GetTableName();
                if (tableName != null && tableName.StartsWith("AspNet"))
                {
                    entityType.SetTableName(tableName.Replace("AspNet", "App"));
                    //entityType.SetTableName(tableName.Substring(6));
                }
            }
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = new CancellationToken())
        {

            UpdateAudiableProperty(
                EntityState.Added,
                AudiablePropertyConstants.CreatedDate,
                DateTime.Now
            );

            UpdateAudiableProperty(
                EntityState.Modified,
                AudiablePropertyConstants.ModifiedDate,
                DateTime.Now
            );


            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private void UpdateAudiableProperty(EntityState state, string propertyName, object value)
        {
            var entities = ChangeTracker.Entries().Where(x => x.State == state);

            foreach (var entityEntry in entities)
            {
                var property = entityEntry.Entity.GetType().GetProperty(propertyName);

                if (entityEntry.State == state && property != null)
                {
                    property.SetValue(entityEntry.Entity, value);
                }
            }
        }
    }
}
