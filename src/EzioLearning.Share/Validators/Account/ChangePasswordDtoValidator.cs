using EzioLearning.Share.Dto.Account;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace EzioLearning.Share.Validators.Account
{
    public class ChangePasswordDtoValidator : AbstractValidator<ChangePasswordDto>
    {
        public ChangePasswordDtoValidator(IStringLocalizer<ChangePasswordDtoValidator> localizer)
        {

            const int passwordMinLength = 8;
            const int passwordMaxLength = 32;

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage(localizer.GetString("PasswordEmpty"))
                .MinimumLength(passwordMinLength)
                .WithMessage(localizer.GetString("PasswordMinimumLength", passwordMinLength))
                .MaximumLength(passwordMaxLength)
                .WithMessage(localizer.GetString("PasswordMaximumLength", passwordMaxLength));
            
            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage(localizer.GetString("PasswordEmpty"))
                .MinimumLength(passwordMinLength)
                .WithMessage(localizer.GetString("PasswordMinimumLength", passwordMinLength))
                .MaximumLength(passwordMaxLength)
                .WithMessage(localizer.GetString("PasswordMaximumLength", passwordMaxLength))
                .NotEqual(x =>x.Password).WithMessage(localizer.GetString("NewPasswordAllow"));

            RuleFor(x => x.ConfirmNewPassword)
                .Equal(x => x.NewPassword).WithMessage(localizer.GetString("ConfirmPasswordNotMatch"));
        }
    }
}
