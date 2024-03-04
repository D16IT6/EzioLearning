namespace EzioLearning.Share.Dto.Auth;

public class ForgotPasswordDto
{
    public string? Email { get; set; }
    public string? ClientConfirmUrl { get; set; }
}