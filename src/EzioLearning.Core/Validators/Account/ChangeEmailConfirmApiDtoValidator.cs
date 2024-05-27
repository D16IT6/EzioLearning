using EzioLearning.Core.Dto.Account;
using EzioLearning.Domain.Entities.Identity;
using EzioLearning.Share.Validators.Account;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;

namespace EzioLearning.Core.Validators.Account
{
    public class ChangeEmailConfirmApiDtoValidator : AbstractValidator<ChangeEmailConfirmApiDto>
    {
        public ChangeEmailConfirmApiDtoValidator(UserManager<AppUser> userManager, IStringLocalizer<ChangeEmailConfirmApiDtoValidator> localizer, ChangeEmailConfirmDtoValidator changeEmailConfirmDtoValidator)
        {
            Include(changeEmailConfirmDtoValidator);

            RuleFor(x => x.UserId)
                .Must(x =>
                {
                    if (string.IsNullOrEmpty(x)) return false;
                    return userManager.FindByIdAsync(x).Result != null;
                }).WithMessage(localizer.GetString("UserNotExist"));


            RuleFor(x => x.Email)
                .Must(email =>
                {
                    if (string.IsNullOrEmpty(email)) return false;
                    var user = userManager.FindByEmailAsync(email).Result;
                    return user == null;
                }).WithMessage(localizer.GetString("EmailExist"));
        }
    }
}
