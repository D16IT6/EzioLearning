using EzioLearning.Domain.Entities.Learning;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EzioLearning.Infrastructure.DbContext.EntityConfigurations.Learning;

internal class CourseLessonConfiguration : IEntityTypeConfiguration<CourseLesson>
{
    public void Configure(EntityTypeBuilder<CourseLesson> builder)
    {
        builder.Property(x => x.Name).HasMaxLength(50).IsUnicode();
        builder.Property(x => x.VideoPath).HasMaxLength(250).IsUnicode();
        builder.Property(x => x.SlidePath).HasMaxLength(250).IsUnicode();
        builder.Property(x => x.Attachment).HasMaxLength(250).IsUnicode();
    }
}