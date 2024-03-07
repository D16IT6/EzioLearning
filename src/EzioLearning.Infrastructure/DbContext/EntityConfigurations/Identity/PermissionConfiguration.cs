using EzioLearning.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EzioLearning.Infrastructure.DbContext.EntityConfigurations.Identity
{
    public class PermissionConfiguration : IEntityTypeConfiguration<AppPermission>
    {
        public void Configure(EntityTypeBuilder<AppPermission> builder)
        {
            builder.Property(x => x.Name).HasMaxLength(50).IsUnicode();
            builder.Property(x => x.DisplayName).HasMaxLength(50).IsUnicode();
            builder.HasMany(p => p.Users)
                .WithMany(u => u.Permissions)
                .UsingEntity(
                    "UserPermissions",
                    l => l.HasOne(typeof(AppUser)).WithMany().HasForeignKey("UserId")
                        .HasPrincipalKey(nameof(AppUser.Id)),
                    r => r.HasOne(typeof(AppPermission)).WithMany().HasForeignKey("PermissionId")
                        .HasPrincipalKey(nameof(AppPermission.Id)),
                    j => j.HasKey("UserId", "PermissionId"));

        }
    }
}
