using EzioLearning.Core.Dto.Account;
using EzioLearning.Domain.Entities.Identity;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace EzioLearning.Core.Validators.Account
{
    public class ChangeEmailApiDtoValidator : AbstractValidator<ChangeEmailApiDto>
    {
        public ChangeEmailApiDtoValidator(UserManager<AppUser> userManager)
        {

            RuleFor(x => x.CurrentEmail)
                .NotEmpty().WithMessage("Email hiện tại không được để trống")
                .EmailAddress().WithMessage("Định dạng email không hợp lệ")
                .Must(email =>
                {
                    if (string.IsNullOrEmpty(email)) return false;
                    var user = userManager.FindByEmailAsync(email).Result;
                    return user != null;
                }).WithMessage("Email không có trong hệ thống, vui lòng thử lại")
                .Must(email =>
                {
                    if (string.IsNullOrEmpty(email)) return false;
                    var user = userManager.FindByEmailAsync(email).Result;
                    return user != null && userManager.IsEmailConfirmedAsync(user).Result;
                }).WithMessage("Email cũ của bạn chưa được xác thực, vui lòng thử lại");


            RuleFor(x => x.NewEmail
                    )
                .NotEmpty().WithMessage("Email mới không được để trống")
                .EmailAddress().WithMessage("Định dạng email không hợp lệ")
                .Must(email =>
                {
                    if (string.IsNullOrEmpty(email)) return false;
                    var user = userManager.FindByEmailAsync(email).Result;
                    return user == null;
                }).WithMessage($"Email mới đã tồn tại, vui lòng thử lại")
                .NotEqual(x => x.CurrentEmail).WithMessage("Email mới không được trùng email cũ");

            RuleFor(x => x.ClientUrl)
                .NotEmpty().WithMessage("Url không được để trống");
        }
    }
}
