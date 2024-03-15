using EzioLearning.Share.Dto.Account;
using FluentValidation;

namespace EzioLearning.Share.Validators.Account
{
    public class ChangePasswordDtoValidator : AbstractValidator<ChangePasswordDto>
    {
        public ChangePasswordDtoValidator()
        {

            RuleFor(x => x.Password)
                .MinimumLength(8).WithMessage("Mật khẩu ít nhất 8 ký tự")
                .MaximumLength(32).WithMessage("Mật khẩu không được quá 32 ký tự");

            RuleFor(x => x.NewPassword)
                .MinimumLength(8).WithMessage("Mật khẩu mới ít nhất 8 ký tự")
                .MaximumLength(32).WithMessage("Mật khẩu mới không được quá 32 ký tự")
                .NotEqual(x=>x.Password).WithMessage("Mật khẩu mới phải khác mật khẩu cũ");

            RuleFor(x => x.ConfirmNewPassword)
                .Equal(x => x.NewPassword).WithMessage("Mật khẩu xác nhận cần trùng khớp");
        }
    }
}
