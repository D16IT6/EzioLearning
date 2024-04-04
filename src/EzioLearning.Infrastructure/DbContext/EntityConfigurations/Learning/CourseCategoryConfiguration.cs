﻿using EzioLearning.Domain.Entities.Learning;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EzioLearning.Infrastructure.DbContext.EntityConfigurations.Learning;

internal class CourseCategoryConfiguration : IEntityTypeConfiguration<CourseCategory>
{
    public void Configure(EntityTypeBuilder<CourseCategory> builder)
    {
        builder.Property(x => x.Image).HasMaxLength(250).IsUnicode();
    }
}