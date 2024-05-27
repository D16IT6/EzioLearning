using Microsoft.AspNetCore.Components.Forms;

namespace EzioLearning.Share.Dto.User;

public class UserCreateDto
{
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string? UserName { get; init; }

    public string? Email { get; init; }
    public string? Password { get; init; }
    public string? ConfirmPassword { get; init; }
    public string? PhoneNumber { get; init; }

    public DateTime? DateOfBirth { get; init; } = DateTime.UtcNow.AddYears(-10);

    protected IBrowserFile? Avatar { get; init; }

    public string? LoginProvider { get; init; }
    public string? ProviderName { get; init; }
    public string? ProviderKey { get; init; }
    public Guid[] RoleIds { get; init; } = [];

}