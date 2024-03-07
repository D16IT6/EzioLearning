using Microsoft.AspNetCore.Identity;

namespace EzioLearning.Domain.Entities.Identity;

public class AppRole : IdentityRole<Guid>
{
    public string DisplayName { get; set; } = string.Empty;
    public override string? ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();

}