using EzioLearning.Domain.Entities.Translation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EzioLearning.Infrastructure.DbContext.EntityConfigurations.Translation
{
    public class CourseCategoryTranslationConfiguration : IEntityTypeConfiguration<CourseCategoryTranslation>
    {
        public void Configure(EntityTypeBuilder<CourseCategoryTranslation> builder)
        {
            builder.Property(x => x.Name).HasMaxLength(50).IsUnicode();

            builder.HasKey(x => new { x.CultureId, x.CourseCategoryId });
        }
    }
}
