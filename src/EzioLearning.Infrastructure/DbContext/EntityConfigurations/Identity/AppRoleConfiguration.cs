using EzioLearning.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EzioLearning.Infrastructure.DbContext.EntityConfigurations.Identity;

internal class AppRoleConfiguration : IEntityTypeConfiguration<AppRole>
{
    public void Configure(EntityTypeBuilder<AppRole> builder)
    {
        builder.Property(x => x.Name).IsRequired().HasMaxLength(16).IsUnicode();

        builder.Property(x => x.DisplayName).IsRequired().HasMaxLength(16).IsUnicode();

        builder.Property(x => x.NormalizedName).IsRequired().HasMaxLength(16).IsUnicode();
    }
}