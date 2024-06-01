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
        UserManager<AppUser> userManager,
        CourseCreateDtoValidator courseCreateDtoValidator)
    {
        Include(courseCreateDtoValidator);
        var availableUserIds = userManager.Users.Select(x => x.Id);

        var availableCourseCategoriesIds =
            courseCategoryRepository.GetAllAsync().Result
                .Where(x => x.IsActive)
                .Select(x => x.Id).ToList();

        RuleFor(x => x.CourseCategoryIds)
            .Must(inputCourseCategories => !inputCourseCategories.Except(availableCourseCategoriesIds).Any())
            .WithMessage("Danh mục không tồn tại");

        //RuleFor(x => x.CreatedBy)
        //    .Must(x => availableUserIds.Contains(x)).WithMessage("Giáo viên không tồn tại");
    }
}