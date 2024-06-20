using EzioLearning.Share.Dto.Learning.Course;
using EzioLearning.Share.Utils;
using EzioLearning.Share.Validators.Common;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace EzioLearning.Share.Validators.Learning.Course
{
    public class CourseLectureCreateDtoValidator : AbstractValidator<CourseLectureCreateDto>
    {
        public CourseLectureCreateDtoValidator(IStringLocalizer<CourseLectureCreateDtoValidator> localizer)
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(localizer.GetString("NameEmpty"))
                .MaximumLength(200).WithMessage("Tên không dài quá 200 ký tự")
                .MinimumLength(5).WithMessage("Tên không ngắn hơn 5 ký tự");

            RuleFor(x => x.LectureType).IsInEnum().WithMessage("Kiểu file không hợp lệ");
        }
    }
}
