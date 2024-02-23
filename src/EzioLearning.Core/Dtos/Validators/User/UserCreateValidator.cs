using EzioLearning.Core.Dtos.User;
using FluentValidation;
using System.Text.RegularExpressions;
using EzioLearning.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace EzioLearning.Core.Dtos.Validators.User
{
    public class UserCreateValidator : AbstractValidator<UserCreateDto>
    {
        public UserCreateValidator(UserManager<AppUser> userManager)
        {

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("Họ không được để trống")
                //.MinimumLength(5).WithMessage("Họ không được dưới 5 ký tự")
                .MaximumLength(50).WithMessage("Họ không được quá 50 ký tự");




            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Tên không được để trống")
                //.MinimumLength(5).WithMessage("Tên không được dưới 5 ký tự")
                .MaximumLength(50).WithMessage("Tên không được quá 50 ký tự");


            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("Tài khoản không được để trống")
                //.MinimumLength(5).WithMessage("Họ không được dưới 5 ký tự")
                .Must(s =>
                {
                    Regex regex = new("^[a-zA-Z0-9]+$");
                    if (s == null)
                        return false;

                    return regex.IsMatch(s);
                }).WithMessage("Tài khoản chỉ chấp nhận chữ cái và số")
                .Must((userName) =>
                {
                    if (string.IsNullOrEmpty(userName)) return false;
                    var user = userManager.FindByNameAsync(userName).Result;
                    return user == null;
                }).WithMessage("Tên tài khoản đã tồn tại, vui lòng thử lại")
                .MaximumLength(32).WithMessage("Họ không được quá 50 ký tự");


            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email không được để trống")
                .Must((email) =>
                {
                    if (string.IsNullOrEmpty(email)) return false;
                    var user = userManager.FindByEmailAsync(email).Result;
                    return user == null;
                }).WithMessage("Email đã tồn tại, vui lòng thử lại")
                .EmailAddress().WithMessage("Định dạng email không hợp lệ");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Mật khẩu không được để trống")
                .MinimumLength(8).WithMessage("Mật khẩu ít nhất 8 ký tự");

            RuleFor(x => x.ConfirmPassword)
                .Equal(x => x.Password).WithMessage("Mật khẩu xác thực cần trùng khớp");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Số điện thoại không được để trống.");

            RuleFor(x => x.DateOfBirth)
                .NotEmpty().WithMessage("Ngày sinh không được để trống.")
                .Must(BeValidDate).WithMessage("Ngày sinh không hợp lệ.");

        }
        private bool BeValidDate(DateOnly dateOfBirth)
        {
            return dateOfBirth <= DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-18);
        }
    }
}
