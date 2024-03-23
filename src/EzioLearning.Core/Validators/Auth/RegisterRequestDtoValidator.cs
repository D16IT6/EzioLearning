using System.Text.RegularExpressions;
using EzioLearning.Core.Dto.Auth;
using EzioLearning.Domain.Entities.Identity;
using EzioLearning.Share.Validators.Common;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;

namespace EzioLearning.Core.Validators.Auth;

public class RegisterRequestDtoValidator : AbstractValidator<RegisterRequestApiDto>
{
    public RegisterRequestDtoValidator(UserManager<AppUser> userManager,IStringLocalizer<RegisterRequestDtoValidator> localizer)
    {
        const int nameMaxLength = 50;
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage(localizer.GetString("FirstNameEmpty"))
            .MaximumLength(50).WithMessage(localizer.GetString("FirstNameMaximumLength", nameMaxLength));


        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage(localizer.GetString("LastNameEmpty"))
            .MaximumLength(50).WithMessage(localizer.GetString("LastNameMaximumLength", nameMaxLength));

        const int userNameMinLength = 6;
        const int userNameMaxLength = 32;
        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage(localizer.GetString("UserNameEmpty"))
            .Must(s =>
            {
                Regex regex = new("^[a-zA-Z0-9]+$");
                return s != null && regex.IsMatch(s);
            }).WithMessage(localizer.GetString("UserNameNotMatchRegex"))
            .MinimumLength(userNameMinLength).WithMessage(localizer.GetString("UserNameMinimumLength", userNameMinLength))
            .MaximumLength(32).WithMessage(localizer.GetString("UserNameMaximumLength", userNameMaxLength))
            .Must(userName =>
            {
                if (string.IsNullOrEmpty(userName)) return false;
                var user = userManager.FindByNameAsync(userName).Result;
                return user == null;
            }).WithMessage(localizer.GetString("UserNameExist"));


        RuleFor(x => x.Email)
            .EmailAddress().WithMessage(localizer.GetString("EmailNotValid"))
            .Must(email =>
            {
                if (string.IsNullOrEmpty(email)) return false;
                var user = userManager.FindByEmailAsync(email).Result;
                return user == null;
            }).WithMessage(localizer.GetString("EmailExist"));

        const int passwordMinLength = 8;
        const int passwordMaxLength = 32;

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage(localizer.GetString("PasswordEmpty"))
            .MinimumLength(passwordMinLength).WithMessage(localizer.GetString("PasswordMinimumLength", passwordMinLength))
            .MaximumLength(passwordMaxLength).WithMessage(localizer.GetString("PasswordMaximumLength", passwordMaxLength));

        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.Password).WithMessage(localizer.GetString("ConfirmPasswordNotMatch"));

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage(localizer.GetString("PhoneNumberEmpty"));

        int minYear = 10, maxYear = 100;
        RuleFor(x => x.DateOfBirth)
            .NotEmpty().WithMessage(localizer.GetString("DateOfBirthEmpty"))
            .Must(x => x.BeValidDate(10, 100)).WithMessage(localizer.GetString("DateOfBirthRange", minYear, maxYear));
    }
}