using EzioLearning.Domain.Entities.Learning;
using Microsoft.AspNetCore.Identity;

namespace EzioLearning.Domain.Entities.Identity;

public class AppUser : IdentityUser<Guid>
{
    [PersonalData] public string? FirstName { get; set; }

    [PersonalData] public string? LastName { get; set; }

    [ProtectedPersonalData] public DateTime? DateOfBirth { get; set; }

    [PersonalData] public string? Avatar { get; set; }

    public bool IsActive { get; set; }

    public string? RefreshToken { get; set; }

    public DateTime? RefreshTokenExpiryTime { get; set; }

    public override string? ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();

    public Trainer? Trainer { get; set; }
    public ICollection<Student> Students { get; set; } = new List<Student>();


    public DateTime CreatedDate { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public Guid ModifiedBy { get; set; }
}