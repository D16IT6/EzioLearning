using EzioLearning.Domain.Common;
using Microsoft.AspNetCore.Identity;

namespace EzioLearning.Domain.Entities.Identity
{
    public class AppUser : IdentityUser<Guid>, IAuditableEntity<Guid>
    {

        [PersonalData]
        public required string FirstName { get; set; }

        [PersonalData]
        public required string LastName { get; set; }

        [ProtectedPersonalData]
        public DateTime? DateOfBirth { get; set; }

        [PersonalData]
        public string? Avatar { get; set; }

        public bool IsActive { get; set; }

        public string? RefreshToken { get; set; }

        public DateTime? RefreshTokenExpiryTime { get; set; }

        public override string? ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();

        public DateTime CreatedDate { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public Guid ModifiedBy { get; set; }
    }
}
