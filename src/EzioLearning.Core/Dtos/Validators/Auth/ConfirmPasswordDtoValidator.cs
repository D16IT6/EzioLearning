using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EzioLearning.Core.Dtos.Auth;
using FluentValidation;

namespace EzioLearning.Core.Dtos.Validators.Auth
{
    public class ConfirmPasswordDtoValidator : AbstractValidator<ConfirmPasswordDto>
    {
        public ConfirmPasswordDtoValidator()
        {
            RuleFor(x => x.Email)
                .EmailAddress()
                .WithMessage("Địa chỉ email không hợp lệ");
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Mật khẩu không được để trống")
                .MinimumLength(8).WithMessage("Mật khẩu ít nhất 8 ký tự")
                .MaximumLength(32).WithMessage("Mật khẩu không được quá 32 ký tự");

            RuleFor(x => x.ConfirmPassword)
                .Equal(x => x.Password).WithMessage("Mật khẩu xác thực cần trùng khớp");
        }
    }
}
