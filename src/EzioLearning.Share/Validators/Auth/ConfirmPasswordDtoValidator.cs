using EzioLearning.Share.Dto.Auth;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace EzioLearning.Share.Validators.Auth;

public class ConfirmPasswordDtoValidator : AbstractValidator<ConfirmPasswordDto>
{
    public ConfirmPasswordDtoValidator(IStringLocalizer<ConfirmPasswordDtoValidator> localizer)
    {
        int minLength = 8, maxLength = 32;

        RuleFor(x => x.Email)
            .EmailAddress()
            .WithMessage(localizer.GetString("EmailNotValid"));
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage(localizer.GetString("PasswordEmpty"))
            .MinimumLength(minLength).WithMessage(localizer.GetString("PasswordMinimumLength", minLength))
            .MaximumLength(maxLength).WithMessage(localizer.GetString("PasswordMaximumLength",maxLength));

        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.Password).WithMessage(localizer.GetString("ConfirmPasswordNotMatch"));
    }
}