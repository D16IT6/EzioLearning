using EzioLearning.Domain.Entities.Learning;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EzioLearning.Infrastructure.DbContext.EntityConfigurations.Learning;

internal class CourseLessonConfiguration : IEntityTypeConfiguration<CourseLecture>
{
    public void Configure(EntityTypeBuilder<CourseLecture> builder)
    {
        builder.Property(x => x.Name).HasMaxLength(250).IsUnicode();
    }
}