using System.Text.RegularExpressions;
using EzioLearning.Core.Dto.Auth;
using EzioLearning.Domain.Entities.Identity;
using EzioLearning.Share.Validators.User;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;

namespace EzioLearning.Core.Validators.Auth
{
    public class UserCreateApiDtoValidator : AbstractValidator<UserCreateApiDto>
    {
        public UserCreateApiDtoValidator(UserManager<AppUser> userManager,
            UserCreateDtoValidator userCreateDtoValidator,
            IStringLocalizer<UserCreateApiDtoValidator> localizer)
        {
            Include(userCreateDtoValidator);

            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage(localizer.GetString("UserNameEmpty"))
                .Must(s =>
                {
                    Regex regex = new("^[a-zA-Z0-9]+$");
                    return s != null && regex.IsMatch(s);
                }).WithMessage(localizer.GetString("UserNameNotMatchRegex"))
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
        }
    }
}
