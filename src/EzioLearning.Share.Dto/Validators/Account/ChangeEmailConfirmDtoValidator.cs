using EzioLearning.Share.Dto.Account;
using FluentValidation;

namespace EzioLearning.Share.Validators.Account
{
    public class ChangeEmailConfirmDtoValidator : AbstractValidator<ChangeEmailConfirmDto>
    {
        public ChangeEmailConfirmDtoValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("Id người dùng không được rỗng");
            RuleFor(x => x.VerifyCode)
                .NotEmpty().WithMessage("Mã xác thực không được rỗng");
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email hiện tại không được để trống")
                .EmailAddress().WithMessage("Định dạng email không hợp lệ");
        }
    }
}
