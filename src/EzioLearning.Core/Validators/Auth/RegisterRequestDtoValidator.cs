﻿using System.Text.RegularExpressions;
using EzioLearning.Core.Dto.Auth;
using EzioLearning.Domain.Entities.Identity;
using EzioLearning.Share.Validators.Common;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace EzioLearning.Core.Validators.Auth;

public class RegisterRequestDtoValidator : AbstractValidator<RegisterRequestApiDto>
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
                return s != null && regex.IsMatch(s);
            }).WithMessage("Tài khoản chỉ chấp nhận chữ cái và số")
            .MaximumLength(32).WithMessage("Tài khoản không được quá 50 ký tự")
            .Must(userName =>
            {
                if (string.IsNullOrEmpty(userName)) return false;
                var user = userManager.FindByNameAsync(userName).Result;
                return user != null;
            }).WithMessage("Tài khoản đã tồn tại hoặc không phù hợp");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email không được để trống")
            .EmailAddress().WithMessage("Định dạng email không hợp lệ")
            .Must(email =>
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
            .Must(x => x.BeValidDate()).WithMessage("Người dùng tuổi chỉ từ 10 tới 100.");
    }
}