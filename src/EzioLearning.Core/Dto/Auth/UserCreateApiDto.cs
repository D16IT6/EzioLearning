using Microsoft.AspNetCore.Http;

namespace EzioLearning.Core.Dto.Auth
{
    public class UserCreateApiDto
    {
        public string? FirstName { get; init; }
        public string? LastName { get; init; }
        public string? UserName { get; init; }

        public string? Email { get; init; }
        public string? Password { get; init; }
        public string? ConfirmPassword { get; init; }
        public string? PhoneNumber { get; init; }

        public DateTime? DateOfBirth { get; init; } = DateTime.UtcNow.AddYears(-10);

        public IFormFile? Avatar { get; init; }

        public string? LoginProvider { get; init; }
        public string? ProviderName { get; init; }
        public string? ProviderKey { get; init; }
        public Guid[] RoleIds { get; init; } = [];

        
    }
}
