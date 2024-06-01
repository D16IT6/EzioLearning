using EzioLearning.Share.Dto.Learning.Course;
using FluentValidation;

namespace EzioLearning.Share.Validators.Learning.Course
{
    public class CourseCreateDtoValidator : AbstractValidator<CourseCreateDto>
    {
        public CourseCreateDtoValidator()
        {
            RuleFor(x => x.Level)
                .IsInEnum().WithMessage("Level không hợp lệ");

            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Nội dung không được để trống");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Tên khoá học không được để trống")
                .MinimumLength(10).WithMessage("Tên không được ngắn hơn 10 ký tự")
                .MaximumLength(250).WithMessage("Tên không dài quá 250 ký tự");

        }

    }
}
