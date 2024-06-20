using EzioLearning.Share.Dto.Learning.Course;
using EzioLearning.Share.Validators.Common;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace EzioLearning.Share.Validators.Learning.Course
{
    public class CourseCreateDtoValidator : AbstractValidator<CourseCreateDto>
    {
        public CourseCreateDtoValidator(IStringLocalizer<CourseCreateDtoValidator> localizer)
        {
            RuleFor(x => x.Level)
                .IsInEnum().WithMessage("Level không hợp lệ");

            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Nội dung không được để trống");

            RuleFor(x => x.Name)
                .MinimumLength(10).WithMessage("Tên không được ngắn hơn 10 ký tự")
                .MaximumLength(250).WithMessage("Tên không dài quá 250 ký tự");

            RuleFor(x => x.PosterImage)
                .Must(x =>
                {
                    if (x == null) return true;
                    var extension = Path.GetExtension(x.Name);
                    return FileConstants.ImageAcceptTypes.Contains(extension);
                })
                .WithMessage("Định dạng ảnh không được chấp nhận")
                .Must(x => 
                    x is null or { Size: <= FileConstants.ImageUploadLimit })
                .WithMessage("Dung lượng ảnh quá lớn");

            RuleFor(x=>x.CourseCategoryIds)
                .Must(x => x.Any()).WithMessage("Danh mục không được để trống");

            RuleFor(x => x.Level).IsInEnum().WithMessage("Trình độ không hợp lệ");
        }

    }
}
