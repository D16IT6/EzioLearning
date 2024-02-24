using EzioLearning.Core.Dtos.User;
using FluentValidation;
using System.Text.RegularExpressions;
using EzioLearning.Core.Dtos.Auth;
using EzioLearning.Core.Dtos.Validators.Auth;
using EzioLearning.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace EzioLearning.Core.Dtos.Validators.User
{
    public class UserCreateDtoValidator : AbstractValidator<UserCreateDto>
    {
        public UserCreateDtoValidator(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            var actuallyRoleListIds = roleManager.Roles.Select(x => x.Id).ToList();
            Include(new RegisterRequestDtoValidator(userManager));
            RuleFor(x => x.RoleIds)
                .NotEmpty().WithMessage("Quyền không được để trống")
                .Must(roleList => roleList.Except(actuallyRoleListIds).Any())
                .WithMessage("Có role không tồn tại trong hệ thống, vui lòng xem lại");
        }

    }
}
