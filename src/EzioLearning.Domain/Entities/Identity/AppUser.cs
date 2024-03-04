using System.ComponentModel.DataAnnotations.Schema;
using EzioLearning.Domain.Entities.Learning;
using Microsoft.AspNetCore.Identity;

namespace EzioLearning.Domain.Entities.Identity;

public class AppUser : IdentityUser<Guid>
{
    [PersonalData] public required string FirstName { get; set; }

    [PersonalData] public required string LastName { get; set; }

    [ProtectedPersonalData] public DateOnly? DateOfBirth { get; set; }

    [PersonalData] public required string Avatar { get; set; }

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
    public override bool LockoutEnabled { get; set; } = false;
    public override bool EmailConfirmed { get; set; } = true;
    public override bool PhoneNumberConfirmed { get; set; } = true;

    [NotMapped] public string FullName => $"{FirstName} {LastName}";
}