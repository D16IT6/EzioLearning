﻿using EzioLearning.Core.Dtos.Auth;
using EzioLearning.Domain.Entities.Identity;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using System.Text.RegularExpressions;

namespace EzioLearning.Core.Dtos.Validators.Auth
{
    public class RegisterRequestDtoValidator : AbstractValidator<RegisterRequestDto>
    {
        public RegisterRequestDtoValidator(UserManager<AppUser> userManager)
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
                .MaximumLength(32).WithMessage("Họ không được quá 50 ký tự")
                .Must((userName) =>
                {
                    if (string.IsNullOrEmpty(userName)) return false;
                    var user = userManager.FindByNameAsync(userName).Result;
                    return user == null;
                }).WithMessage("Tên tài khoản đã tồn tại, vui lòng thử lại")
                .MaximumLength(32).WithMessage("Họ không được quá 50 ký tự");


            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email không được để trống")
                .EmailAddress().WithMessage("Định dạng email không hợp lệ")
                .Must((email) =>
                {
                    if (string.IsNullOrEmpty(email)) return false;
                    var user = userManager.FindByEmailAsync(email).Result;
                    return user == null;
                }).WithMessage("Email đã tồn tại, vui lòng thử lại");

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
                .Must(BeValidDate).WithMessage("Người dùng tuổi chỉ từ 10 tới 100.");


        }
        private bool BeValidDate(DateOnly dateOfBirth)
        {
            return dateOfBirth <= DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-10) && dateOfBirth >= DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-100);
        }
    }
}
