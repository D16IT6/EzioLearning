using EzioLearning.Share.Dto.Account;
using EzioLearning.Share.Validators.Common;
using FluentValidation;
using System;
using Microsoft.Extensions.Localization;

namespace EzioLearning.Share.Validators.Account
{
    public class AccountInfoDtoValidator : AbstractValidator<AccountInfoDto>
    {
        public AccountInfoDtoValidator(IStringLocalizer<AccountInfoDtoValidator> localizer)
        {

            const int nameMaxLength = 50;
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage(localizer.GetString("FirstNameEmpty"))
                .MaximumLength(50).WithMessage(localizer.GetString("FirstNameMaximumLength", nameMaxLength));

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage(localizer.GetString("LastNameEmpty"))
                .MaximumLength(50).WithMessage(localizer.GetString("LastNameMaximumLength", nameMaxLength));

            RuleFor(x => x.Email)
                .EmailAddress().WithMessage(localizer.GetString("EmailNotValid"));


            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage(localizer.GetString("PhoneNumberEmpty"));

            const int minYear = 10;
            const int maxYear = 100;
            RuleFor(x => x.DateOfBirth)
                .NotEmpty().WithMessage(localizer.GetString("DateOfBirthEmpty"))
                .Must(x => x.BeValidDate(10, 100))
                .WithMessage(localizer.GetString("DateOfBirthRange", minYear, maxYear));


        }
    }
}
