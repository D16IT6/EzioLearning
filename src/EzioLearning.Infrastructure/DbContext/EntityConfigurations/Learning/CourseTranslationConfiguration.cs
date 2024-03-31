using EzioLearning.Domain.Entities.Translation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EzioLearning.Infrastructure.DbContext.EntityConfigurations.Learning
{
    internal class CourseTranslationConfiguration : IEntityTypeConfiguration<CourseTranslation>
    {
        public void Configure(EntityTypeBuilder<CourseTranslation> builder)
        {
            builder.Property(x => x.Name).HasMaxLength(50).IsUnicode();
            builder.Property(x => x.Description).HasMaxLength(500).IsUnicode();

        }
    }
}
