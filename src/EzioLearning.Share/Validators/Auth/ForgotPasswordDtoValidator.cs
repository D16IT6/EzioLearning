using EzioLearning.Share.Dto.Auth;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace EzioLearning.Share.Validators.Auth;

public class ForgotPasswordDtoValidator : AbstractValidator<ForgotPasswordDto>
{
    public ForgotPasswordDtoValidator(IStringLocalizer<ForgotPasswordDtoValidator> localizer)
    {

        const int minLength = 5;
        RuleFor(x => x.Email)
            .EmailAddress()
            .WithMessage(localizer.GetString("EmailNotValid"));
        RuleFor(x => x.ClientConfirmUrl)
            .NotEmpty().WithMessage(localizer.GetString("UrlEmpty"))
            .MinimumLength(minLength).WithMessage(localizer.GetString("UrlMinimumLength",minLength));
    }
}