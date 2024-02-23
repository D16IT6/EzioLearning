using System.ComponentModel.DataAnnotations.Schema;
using EzioLearning.Domain.Entities.Learning;
using Microsoft.AspNetCore.Identity;

namespace EzioLearning.Domain.Entities.Identity;

public class AppUser : IdentityUser<Guid>
{
    [PersonalData] public string? FirstName { get; set; }

    [PersonalData] public string? LastName { get; set; }

    [ProtectedPersonalData] public DateOnly? DateOfBirth { get; set; }

    [PersonalData] public string? Avatar { get; set; }

    public bool IsActive { get; set; }

    public string? RefreshToken { get; set; }

    public DateTime? RefreshTokenExpiryTime { get; set; }

    public override string? ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();

    public ICollection<Student> Students { get; set; } = new List<Student>();

    public ICollection<Course> Courses { get; set; } = new List<Course>();

    public string? CacheKey { get; set; }
    public DateTime CreatedDate { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public Guid ModifiedBy { get; set; }

    [NotMapped] public string FullName => $"{FirstName} {LastName}";
}
