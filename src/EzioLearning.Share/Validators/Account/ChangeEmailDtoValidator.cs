using EzioLearning.Share.Dto.Account;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace EzioLearning.Share.Validators.Account
{
    public class ChangeEmailDtoValidator : AbstractValidator<ChangeEmailDto>
    {
        public ChangeEmailDtoValidator(IStringLocalizer<ChangeEmailDtoValidator> localizer)
        {
            RuleFor(x => x.NewEmail)
                .EmailAddress().WithMessage(localizer.GetString("EmailNotValid"))
                .NotEqual(x=>x.CurrentEmail).WithMessage(localizer.GetString("NewEmailNotEqual"));

            RuleFor(x => x.CurrentEmail)
                .EmailAddress().WithMessage(localizer.GetString("EmailNotValid"));

            RuleFor(x => x.ClientUrl)
                .NotEmpty().WithMessage(localizer.GetString("UrlEmpty"));

        }
    }
}
