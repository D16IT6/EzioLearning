using System.ComponentModel.DataAnnotations;

namespace EzioLearning.Share.Dto.Auth;

public class LoginRequestDto
{
    public string? UserName { get; set; }

    public string? Password { get; set; }
}