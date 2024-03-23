using System.Text.RegularExpressions;
using EzioLearning.Share.Dto.Auth;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace EzioLearning.Share.Validators.Auth;

public class LoginRequestDtoValidator : AbstractValidator<LoginRequestDto>
{
    public LoginRequestDtoValidator(IStringLocalizer<LoginRequestDtoValidator> localizer)
    {
        const int minLength = 6;
        const int maxLength = 32;
        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage(_ => localizer.GetString("UserNameEmpty"))
            .Must(s =>
            {

                Regex regex = new("^[a-zA-Z0-9]+$");
                return s != null && regex.IsMatch(s);

            }).WithMessage(localizer.GetString("UserNameNotMatchRegex"))

            .MinimumLength(minLength).WithMessage(localizer.GetString("PasswordMinimumLength", minLength))
            .MaximumLength(maxLength).WithMessage(localizer.GetString("UserNameMaximumLength", maxLength));


        RuleFor(x => x.Password)
            .NotEmpty().WithMessage(localizer.GetString("PasswordEmpty"))
            .MinimumLength(minLength).WithMessage(localizer.GetString("PasswordMinimumLength",minLength))
            .MaximumLength(maxLength).WithMessage(localizer.GetString("PasswordMaximumLength", maxLength));
    }
}