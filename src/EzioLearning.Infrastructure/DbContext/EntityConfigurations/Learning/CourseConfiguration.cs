﻿
using EzioLearning.Domain.Entities.Learning;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EzioLearning.Infrastructure.DbContext.EntityConfigurations.Learning
{
    internal class CourseConfiguration : IEntityTypeConfiguration<Course>

    {
        public void Configure(EntityTypeBuilder<Course> builder)
        {
            builder.Property(x => x.Name).HasMaxLength(50).IsUnicode();

            builder
                .HasMany(x => x.Categories)
                .WithMany(x => x.Courses)
                .UsingEntity(x => x.ToTable("CourseInCategories"));

        }
    }
}