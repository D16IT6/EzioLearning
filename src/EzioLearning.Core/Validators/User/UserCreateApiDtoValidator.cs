using EzioLearning.Domain.Entities.Identity;
using EzioLearning.Share.Dto.User;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using System.Text.RegularExpressions;
using EzioLearning.Share.Validators.User;

namespace EzioLearning.Core.Validators.User;

public class UserCreateApiDtoValidator : AbstractValidator<UserCreateDto>
{
    public UserCreateApiDtoValidator(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager,UserCreateDtoValidator userCreateDtoValidator)
    {
        Include(userCreateDtoValidator);

        RuleFor(x => x.UserName)
            .Must(s =>
            {
                Regex regex = new("^[a-zA-Z0-9]+$");
                if (s == null)
                    return false;

                return regex.IsMatch(s);
            }).WithMessage("Tài khoản chỉ chấp nhận chữ cái và số")
            .Must(userName =>
            {
                if (string.IsNullOrEmpty(userName)) return false;
                var user = userManager.FindByNameAsync(userName).Result;
                return user == null;
            }).WithMessage("Tên tài khoản đã tồn tại, vui lòng thử lại");

        RuleFor(x => x.Email)
            .Must(email =>
            {
                if (string.IsNullOrEmpty(email)) return false;
                var user = userManager.FindByEmailAsync(email).Result;
                return user == null;
            }).WithMessage("Email đã tồn tại, vui lòng thử lại");
        
        var actuallyRoleListIds = roleManager.Roles.Select(x => x.Id).ToList();
        RuleFor(x => x.RoleIds)
            .Must(roleList => !roleList.Except(actuallyRoleListIds).Any())
            .WithMessage("Có role không tồn tại trong hệ thống, vui lòng xem lại");
    }
}