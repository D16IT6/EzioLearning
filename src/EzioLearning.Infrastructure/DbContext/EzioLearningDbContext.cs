using EzioLearning.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using EzioLearning.Domain.Common;
using EzioLearning.Domain.Entities.Learning;

namespace EzioLearning.Infrastructure.DbContext
{
    public class EzioLearningDbContext(DbContextOptions options) : IdentityDbContext<AppUser, AppRole, Guid>(options)
    {
        #region Tables
        public DbSet<Course> Courses { get; set; }
        public DbSet<CourseCategory> CourseCategories { get; set; }
        public DbSet<CourseRating> CourseRatings { get; set; }
        public DbSet<CourseLesson> CourseLessons { get; set; }
        public DbSet<LessonComment> LessonComments { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Trainer> Trainers { get; set; }

        #endregion

        #region Model Creating

        protected override void OnModelCreating(ModelBuilder builder)
        {

            base.OnModelCreating(builder);

            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            ConfigIdentity(builder);


        }

        public void ConfigIdentity(ModelBuilder builder, string prefix = "App", string schema = "Auth")
        {
            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                var tableName = entityType.GetTableName();
                if (tableName != null && tableName.StartsWith("AspNet"))
                {
                    entityType.SetTableName(tableName.Replace("AspNet", prefix));
                    entityType.SetSchema(schema);
                }
            }
        }

        #endregion

        #region SaveChanges

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = new CancellationToken())
        {

            UpdateAuditableProperty(
                EntityState.Added,
                AuditablePropertyConstants.CreatedDate,
                DateTime.Now
            );

            UpdateAuditableProperty(
                EntityState.Modified,
                AuditablePropertyConstants.ModifiedDate,
                DateTime.Now
            );


            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private void UpdateAuditableProperty(EntityState state, string propertyName, object value)
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

        #endregion
    }
}
