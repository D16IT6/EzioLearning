using EzioLearning.Core.Dtos.Auth;
using FluentValidation;

namespace EzioLearning.Core.Dtos.Validators.Auth
{
	public class ForgotPasswordDtoValidator : AbstractValidator<ForgotPasswordDto>
	{
		public ForgotPasswordDtoValidator()
		{
			RuleFor(x => x.Email)
				.EmailAddress()
				.WithMessage("Địa chỉ email không hợp lệ");
            RuleFor(x => x.ClientConfirmUrl)
                .NotEmpty().WithMessage("Url không được để trống")
                .MinimumLength(5).WithMessage("Url phải dài hơn 5 ký tự");

        }
	}
}
