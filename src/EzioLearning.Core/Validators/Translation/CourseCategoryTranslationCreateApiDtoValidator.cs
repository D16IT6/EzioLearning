using System.Globalization;
using EzioLearning.Core.Dto.Translation;
using EzioLearning.Core.Repositories.Learning;
using EzioLearning.Core.Repositories.System;
using EzioLearning.Domain.Entities.Learning;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace EzioLearning.Core.Validators.Translation
{
    public class CourseCategoryTranslationCreateApiDtoValidator : AbstractValidator<CourseCategoryTranslationCreateApiDto>
    {
        private readonly ICourseCategoryRepository _courseCategoryRepository;
        public CourseCategoryTranslationCreateApiDtoValidator(
            ICourseCategoryRepository courseCategoryRepository, 
            IStringLocalizer<CourseCategoryTranslationCreateApiDtoValidator> localizer, 
            ICultureRepository? cultureRepository)
        {
            _courseCategoryRepository = courseCategoryRepository;


            var cultures = cultureRepository?.GetAllAsync().Result.Select(x => x.Id);


            RuleFor(x => x.Culture)
                .Must(culture =>
                {
                    if (string.IsNullOrEmpty(culture)) culture = CultureInfo.CurrentCulture.Name;

                    return cultures!.Contains(culture);
                })
                .WithMessage(localizer.GetString("CultureNotFound"));

            RuleFor(x => x.Id)
                .Must(courseCategoryId =>
                {
                    var courseCategory = courseCategoryRepository.Find(x=> x.Id == courseCategoryId).Result.FirstOrDefault();
                    return courseCategory != null;
                })
                .WithMessage(localizer.GetString("CourseCategoryNotFound"));

            RuleFor(x => x)
                .Must(x => CheckExistCultureInCourseCategory(x).Result)
                .WithName("CultureInCourseCategory")
                .WithMessage(localizer.GetString("ExistedCultureInCourseCategory"));

        }

        private async Task<bool> CheckExistCultureInCourseCategory(CourseCategoryTranslationCreateApiDto model)
        {
            if (string.IsNullOrEmpty(model.Culture)) model.Culture = CultureInfo.CurrentCulture.Name;
            var currentCourseCategory = (await _courseCategoryRepository.Find(x=>x.Id == model.Id, [nameof(CourseCategory.CourseCategoryTranslations)])).FirstOrDefault();

            if (currentCourseCategory == null) return false;

            var currentCultures = currentCourseCategory.CourseCategoryTranslations
                .Select(x => x.CultureId);

            return !currentCultures.Contains(model.Culture);
        }
    }
}
