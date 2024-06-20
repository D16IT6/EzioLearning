using EzioLearning.Core.Dto.Learning.Course;
using EzioLearning.Core.Repositories.Learning;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace EzioLearning.Core.Validators.Course
{
    public class CourseSectionCreateApiDtoValidator : AbstractValidator<CourseSectionCreateApiDto>
    {
        public CourseSectionCreateApiDtoValidator(
            IStringLocalizer<CourseSectionCreateApiDtoValidator> localizer,
            ICourseRepository courseRepository)
        {
            var courseIds = courseRepository.GetAllAsync().Result.AsQueryable().Select(x => x.Id);

            RuleFor(x => x.CourseId)
                .Must(courseId => courseIds.Contains(courseId)).WithMessage("Khoá học không tồn tại!");
        }
    }
}
