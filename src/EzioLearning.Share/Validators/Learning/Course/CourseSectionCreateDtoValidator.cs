using EzioLearning.Share.Dto.Learning.Course;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace EzioLearning.Share.Validators.Learning.Course
{
    public class CourseSectionCreateDtoValidator : AbstractValidator<CourseSectionCreateDto>
    {
        public CourseSectionCreateDtoValidator(IStringLocalizer<CourseSectionCreateDtoValidator> localizer)
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(localizer.GetString("NameEmpty"))
                .MaximumLength(200).WithMessage("Tên không quá 200 ký tự")
                .MinimumLength(5).WithMessage("Tên phải hơn 5 ký tự");
        }
    }
}
