using EzioLearning.Share.Dto.Account;
using EzioLearning.Share.Validators.Common;
using FluentValidation;

namespace EzioLearning.Share.Validators.Account
{
    public class AccountInfoDtoValidator : AbstractValidator<AccountInfoDto>
    {
        public AccountInfoDtoValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("Họ không được để trống")
                .MaximumLength(50).WithMessage("Họ không được quá 50 ký tự");


            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Tên không được để trống")
                .MaximumLength(50).WithMessage("Tên không được quá 50 ký tự");
            
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email không được để trống")
                .EmailAddress().WithMessage("Định dạng email không hợp lệ");


            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Số điện thoại không được để trống.");

            RuleFor(x => x.DateOfBirth)
                .NotEmpty().WithMessage("Ngày sinh không được để trống.")
                .Must(x => x.BeValidDate()).WithMessage("Người dùng tuổi chỉ từ 10 tới 100.");

        }
    }
}
