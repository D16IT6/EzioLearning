using Microsoft.EntityFrameworkCore;
using EzioLearning.Domain.Entities.System;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EzioLearning.Infrastructure.DbContext.EntityConfigurations.System
{
    public class CultureConfiguration : IEntityTypeConfiguration<Culture>
    {
        public void Configure(EntityTypeBuilder<Culture> builder)
        {
            builder.ToTable($"{nameof(Culture)}s", nameof(System));

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasMaxLength(4).IsUnicode(false);
            builder.Property(x => x.Name).HasMaxLength(50).IsUnicode(false);
        }
    }
}
