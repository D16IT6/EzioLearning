﻿using System.Reflection;
using EzioLearning.Domain.Common;
using EzioLearning.Domain.Entities.Identity;
using EzioLearning.Domain.Entities.Learning;
using EzioLearning.Domain.Entities.System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EzioLearning.Infrastructure.DbContext;

public class EzioLearningDbContext(DbContextOptions options) : IdentityDbContext<AppUser, AppRole, Guid>(options)
{
    #region Tables

    public DbSet<Course> Courses { get; set; } = default!;
    public DbSet<CourseCategory> CourseCategories { get; set; } = default!;
    public DbSet<CourseRating> CourseRatings { get; set; } = default!;
    public DbSet<CourseLesson> CourseLessons { get; set; } = default!;
    public DbSet<LessonComment> LessonComments { get; set; } = default!;
    public DbSet<Student> Students { get; set; } = default!;
    public DbSet<AppPermission> Permissions { get; set; } = default!;
    public DbSet<Culture> Cultures { get; set; } = default!;


    #endregion

    #region Model Creating

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        ConfigureIdentity(builder);

        ConfigureFilters(builder);
    }

    private void ConfigureFilters(ModelBuilder builder)
    {
        builder.Entity<AppUser>().HasQueryFilter(user => user.IsDeleted == false);

    }

    public void ConfigureIdentity(ModelBuilder builder, string prefix = "App", string schema = "Auth")
    {
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            var tableName = entityType.GetTableName();

            if (tableName == null || !tableName.StartsWith("AspNet")) continue;

            entityType.SetTableName(tableName.Replace("AspNet", prefix));
            entityType.SetSchema(schema);
        }
    }

    #endregion

    #region SaveChanges

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = new())
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

            if (entityEntry.State == state && property != null) property.SetValue(entityEntry.Entity, value);
        }
    }

    #endregion
}