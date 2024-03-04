using EzioLearning.Share.Dto.Auth;
using FluentValidation;

namespace EzioLearning.Share.Validators.Auth;

public class ConfirmPasswordDtoValidator : AbstractValidator<ConfirmPasswordDto>
{
    public ConfirmPasswordDtoValidator()
    {
        RuleFor(x => x.Email)
            .EmailAddress()
            .WithMessage("Địa chỉ email không hợp lệ");
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Mật khẩu không được để trống")
            .MinimumLength(8).WithMessage("Mật khẩu ít nhất 8 ký tự")
            .MaximumLength(32).WithMessage("Mật khẩu không được quá 32 ký tự");

        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.Password).WithMessage("Mật khẩu xác thực cần trùng khớp");
    }
}