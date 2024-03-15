using EzioLearning.Core.Dto.Account;
using EzioLearning.Domain.Entities.Identity;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace EzioLearning.Core.Validators.Account
{
    public class ChangeEmailConfirmApiDtoValidator : AbstractValidator<ChangeEmailConfirmApiDto>
    {
        public ChangeEmailConfirmApiDtoValidator(UserManager<AppUser> userManager)
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("Id người dùng không được rỗng")
                .Must(x =>
                {
                    if (string.IsNullOrEmpty(x)) return false;
                    return userManager.FindByIdAsync(x).Result != null;
                }).WithMessage("Người dùng không tồn tại");

            RuleFor(x => x.VerifyCode)
                .NotEmpty().WithMessage("Mã xác thực không được rỗng");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email hiện tại không được để trống")
                .EmailAddress().WithMessage("Định dạng email không hợp lệ")
                .Must(email =>
                {
                    if (string.IsNullOrEmpty(email)) return false;
                    var user = userManager.FindByEmailAsync(email).Result;
                    return user == null;
                }).WithMessage("Email đã có trong hệ thống, vui lòng thử lại");
        }
    }
}
