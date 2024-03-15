using EzioLearning.Share.Dto.Account;
using FluentValidation;

namespace EzioLearning.Share.Validators.Account
{
    public class ChangeEmailDtoValidator : AbstractValidator<ChangeEmailDto>
    {
        public ChangeEmailDtoValidator()
        {
            RuleFor(x => x.NewEmail)
                .NotEmpty().WithMessage("Email mới không được để trống")
                .EmailAddress().WithMessage("Định dạng email không hợp lệ")
                .NotEqual(x=>x.CurrentEmail).WithMessage("Email mới không được trùng email cũ");

            RuleFor(x => x.CurrentEmail)
                .NotEmpty().WithMessage("Email hiện tại không được để trống")
                .EmailAddress().WithMessage("Định dạng email không hợp lệ");

            RuleFor(x => x.ClientUrl)
                .NotEmpty().WithMessage("Url không được để trống");

        }
    }
}
