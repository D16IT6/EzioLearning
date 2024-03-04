using System.ComponentModel.DataAnnotations;

namespace EzioLearning.Share.Dto.Auth;

public class LoginRequestDto
{
    [Display(Name = "Tên đăng nhập")]
    [Required(ErrorMessage = "{0} là bắt buộc")]
    [StringLength(32, ErrorMessage = "{0} chỉ dài từ {2} tới {1} ký tự", MinimumLength = 8)]
    public string? UserName { get; set; }

    [Display(Name = "Mật khẩu")]
    [Required(ErrorMessage = "{0} là bắt buộc")]
    [StringLength(32, ErrorMessage = "{0} chỉ dài từ {2} tới {1} ký tự", MinimumLength = 8)]
    public string? Password { get; set; }
}