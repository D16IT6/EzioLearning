using EzioLearning.Share.Dto.Account;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace EzioLearning.Share.Validators.Account
{
    public class ChangeEmailConfirmDtoValidator : AbstractValidator<ChangeEmailConfirmDto>
    {
        public ChangeEmailConfirmDtoValidator(IStringLocalizer<ChangeEmailConfirmDtoValidator> localizer)
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage(localizer.GetString("UserIdEmpty"));
            RuleFor(x => x.VerifyCode)
                .NotEmpty().WithMessage(localizer.GetString("VerifyCodeEmpty"));
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email hiện tại không được để trống")
                .EmailAddress().WithMessage(localizer.GetString("EmailNotValid"));
        }
    }
}
