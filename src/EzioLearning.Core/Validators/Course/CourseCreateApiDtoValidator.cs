using EzioLearning.Core.Dto.Learning.Course;
using EzioLearning.Core.Repositories.Learning;
using EzioLearning.Domain.Entities.Identity;
using EzioLearning.Share.Validators.Learning.Course;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace EzioLearning.Core.Validators.Course;

public class CourseCreateApiDtoValidator : AbstractValidator<CourseCreateApiDto>
{
    public CourseCreateApiDtoValidator(
        ICourseCategoryRepository courseCategoryRepository,
        ICourseRepository courseRepository,
        UserManager<AppUser> userManager,
        CourseCreateDtoValidator courseCreateDtoValidator)
    {
        Include(courseCreateDtoValidator);

        var availableUserIds = userManager.Users.Select(x => x.Id);

        var availableCourseCategoriesIds =
            courseCategoryRepository.GetAllAsync().Result
                .Where(x => x.IsActive)
                .Select(x => x.Id).ToList();

        var courseNames = (courseRepository.GetAllAsync()).Result.AsQueryable().Select(x => x.Name);

        RuleFor(x => x.Name)
            .MinimumLength(5).WithMessage("Tên khoá học không ngắn hơn 5 ký tự")
            .MaximumLength(200).WithMessage("Tên khoá học không dài quá 200 ký tự")
            .Must(x => !courseNames.Contains(x)).WithMessage("Tên đã tồn tại, vui lòng chọn tên khác");

        RuleFor(x => x.CourseCategoryIds)
            .Must(inputCourseCategories =>
            {
                var except = inputCourseCategories.Except(availableCourseCategoriesIds);
                return !except.Any();
            })
            .WithMessage("Danh mục không có");

        RuleFor(x => x.CreatedBy)
            .Must(x => availableUserIds.Contains(x)).WithMessage("Giáo viên không tồn tại");
    }
}