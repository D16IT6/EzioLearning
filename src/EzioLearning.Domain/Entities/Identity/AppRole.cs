using Microsoft.AspNetCore.Identity;

namespace EzioLearning.Domain.Entities.Identity
{
    public class AppRole : IdentityRole<Guid>
    {
        public required string DisplayName { get; set; }
        public override string? ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();
    }
}
