using EzioLearning.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EzioLearning.Infrastructure.DbContext.EntityConfigurations.Identity
{
    public class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
    {
        public void Configure(EntityTypeBuilder<AppUser> builder)
        {

            builder.Property(x => x.UserName).HasMaxLength(50);
            builder.Property(x => x.NormalizedUserName).HasMaxLength(50);


            builder.Property(x => x.FirstName).HasMaxLength(50).IsUnicode();
            builder.Property(x => x.LastName).HasMaxLength(50).IsUnicode();


            builder.Property(x => x.RefreshToken).HasMaxLength(100);
            builder.Property(x => x.Avatar).HasMaxLength(200);


            builder.Property(x => x.Email).HasMaxLength(32);
            builder.Property(x => x.NormalizedEmail).HasMaxLength(50);
            builder.Property(x => x.PhoneNumber).HasMaxLength(32);

            
        }
    }
}
