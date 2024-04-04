using EzioLearning.Core.Repositories.Learning;
using EzioLearning.Domain.Entities.Identity;
using EzioLearning.Share.Dto.Learning.Course;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace EzioLearning.Core.Validators.Course;

public class CourseCreateDtoValidator : AbstractValidator<CourseCreateDto>
{
    public CourseCreateDtoValidator(ICourseCategoryRepository courseCategoryRepository,
        UserManager<AppUser> userManager)
    {
        var availableUserIds = userManager.Users.Select(x => x.Id);

        var availableCourseCategoriesIds =
            courseCategoryRepository.GetAllAsync().Result
                .Where(x => x.IsActive)
                .Select(x => x.Id).ToList();


        RuleFor(x => x.Level)
            .IsInEnum().WithMessage("Level không hợp lệ");

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Nội dung không được để trống");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Tên khoá học không được để trống")
            .MaximumLength(250).WithMessage("Tên không dài quá 250 ký tự");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Nội dung không được để trống");

        RuleFor(x => x.CourseCategoryIds)
            .Must(inputCourseCategories => !inputCourseCategories.Except(availableCourseCategoriesIds).Any())
            .WithMessage("Danh mục không tồn tại");

        RuleFor(x => x.CreatedBy)
            .Must(x => availableUserIds.Contains(x)).WithMessage("Giáo viên không tồn tại");
    }
}