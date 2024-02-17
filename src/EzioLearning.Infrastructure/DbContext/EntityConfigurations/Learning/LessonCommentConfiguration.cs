using EzioLearning.Domain.Entities.Learning;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EzioLearning.Infrastructure.DbContext.EntityConfigurations.Learning
{
    internal class LessonCommentConfiguration : IEntityTypeConfiguration<LessonComment>
    {
        public void Configure(EntityTypeBuilder<LessonComment> builder)
        {
            builder.Property(x => x.Content).HasMaxLength(4000).IsUnicode();
        }
    }
}
