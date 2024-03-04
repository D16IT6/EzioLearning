using System.Text.RegularExpressions;
using EzioLearning.Share.Dto.Auth;
using EzioLearning.Share.Validators.Common;
using FluentValidation;

namespace EzioLearning.Share.Validators.Auth;

public class RegisterRequestClientValidator : AbstractValidator<RegisterRequestClientDto>
{
    public RegisterRequestClientValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Họ không được để trống")
            .MaximumLength(50).WithMessage("Họ không được quá 50 ký tự");


        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Tên không được để trống")
            .MaximumLength(50).WithMessage("Tên không được quá 50 ký tự");


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


        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email không được để trống")
            .EmailAddress().WithMessage("Định dạng email không hợp lệ");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Mật khẩu không được để trống")
            .MinimumLength(8).WithMessage("Mật khẩu ít nhất 8 ký tự")
            .MaximumLength(32).WithMessage("Mật khẩu không được quá 32 ký tự");

        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.Password).WithMessage("Mật khẩu xác thực cần trùng khớp");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Số điện thoại không được để trống.");

        RuleFor(x => x.DateOfBirth)
            .NotEmpty().WithMessage("Ngày sinh không được để trống.")
            .Must(x => x.BeValidDate()).WithMessage("Người dùng tuổi chỉ từ 10 tới 100.");


        RuleFor(x => x.AllowPolicy).Equal(true).WithMessage("Điều khoản là bắt buộc");
    }
}