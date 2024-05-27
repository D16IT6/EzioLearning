using EzioLearning.Core.Dto.Account;
using EzioLearning.Domain.Entities.Identity;
using EzioLearning.Share.Validators.Account;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;

namespace EzioLearning.Core.Validators.Account
{
    public class ChangeEmailApiDtoValidator : AbstractValidator<ChangeEmailApiDto>
    {
        public ChangeEmailApiDtoValidator(UserManager<AppUser> userManager, 
            IStringLocalizer<ChangeEmailApiDtoValidator> localizer
            ,ChangeEmailDtoValidator changeEmailDtoValidator)
        {
            Include(changeEmailDtoValidator);

            RuleFor(x => x.CurrentEmail)
                .Must(email =>
                {
                    if (string.IsNullOrEmpty(email)) return false;
                    var user = userManager.FindByEmailAsync(email).Result;
                    return user != null;
                }).WithMessage(localizer.GetString("EmailNotExist"))
                .Must(email =>
                {
                    if (string.IsNullOrEmpty(email)) return false;
                    var user = userManager.FindByEmailAsync(email).Result;
                    return user != null && userManager.IsEmailConfirmedAsync(user).Result;
                }).WithMessage(localizer.GetString("EmailNotConfirm"));


            RuleFor(x => x.NewEmail
                    )
                .Must(email =>
                {
                    if (string.IsNullOrEmpty(email)) return false;
                    var user = userManager.FindByEmailAsync(email).Result;
                    return user == null;
                }).WithMessage(localizer.GetString("EmailExist"))
                .NotEqual(x => x.CurrentEmail).WithMessage(localizer.GetString("NewEmailEqual"));
        }
    }
}
