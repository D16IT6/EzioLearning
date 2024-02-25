using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using EzioLearning.Core.Dtos.Auth;
using FluentValidation;

namespace EzioLearning.Core.Dtos.Validators.Auth
{
    public class LoginRequestDtoValidator : AbstractValidator<LoginRequestDto>
    {
        public LoginRequestDtoValidator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("Tài khoản không được để trống")
                .Must(s =>
                {
                    Regex regex = new("^[a-zA-Z0-9]+$");
                    if (s == null)
                        return false;

                    return regex.IsMatch(s);
                }).WithMessage("Tài khoản chỉ chấp nhận chữ cái và số")
                .MaximumLength(32).WithMessage("Họ không được quá 50 ký tự");


            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Mật khẩu không được để trống")
                .MinimumLength(8).WithMessage("Mật khẩu ít nhất 8 ký tự")
                .MaximumLength(32).WithMessage("Mật khẩu không được quá 32 ký tự");

        }
    }
}
