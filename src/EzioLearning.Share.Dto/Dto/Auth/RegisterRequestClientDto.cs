using Microsoft.AspNetCore.Http;

namespace EzioLearning.Share.Dto.Auth;

public class RegisterRequestClientDto
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? UserName { get; set; }

    public string? Email { get; set; }
    public string? Password { get; set; }
    public string? ConfirmPassword { get; set; }
    public string? PhoneNumber { get; set; }

    public DateTime? DateOfBirth { get; set; } = DateTime.UtcNow.AddYears(-10);

    public IFormFile? Avatar { get; set; }

    public string? LoginProvider { get; set; }
    public string? ProviderName { get; set; }
    public string? ProviderKey { get; set; }
    public bool AllowPolicy { get; set; }
}