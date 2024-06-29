using BlazorAnimate;
using EzioLearning.Share.Dto.Learning.Course;
using EzioLearning.Share.Dto.Learning.CourseCategory;
using EzioLearning.Share.Dto.User;
using Microsoft.AspNetCore.Components;
using EzioLearning.Wasm.Services.Interface;

namespace EzioLearning.Wasm.Pages;

public partial class Home
{
    [Inject] private ICourseCategoryService CourseCategoryService { get; set; } = default!;
    [Inject] private ICourseService CourseService { get; set; } = default!;


    private IEnumerable<CourseCategoryViewDto> CourseCategories { get; set; } = [];
    private int CourseCount { get; set; }

    private IEnumerable<TopCourseCategoryDto> TopCourseCategories { get; set; } = [];

    private IEnumerable<CourseViewDto> FeatureCourses { get; set; } = [];
    private IEnumerable<CourseViewDto> TrendingCourses { get; set; } = [];
    private IEnumerable<InstructorViewDto> FeatureInstructors { get; set; } = [];
    private IAnimation? AnimationType { get; set; }
    private TimeSpan AnimationDuration { get; set; }


    protected override void OnInitialized()
    {
        AnimationType = Animations.FadeUp;
        AnimationDuration = TimeSpan.FromSeconds(1);
    }
    
    protected override async Task OnInitializedAsync()
    {
        CourseCategories = (await CourseCategoryService.GetCourseCategories())
            .Where(x => x.ParentId == null).ToList();

        CourseCount = await CourseService.GetCourseCount();

        TopCourseCategories = await CourseService.GetTopCourseCategories(12);

        FeatureCourses = await CourseService.GetFeatureCourses();

        TrendingCourses = await CourseService.GetFeatureCourses();

        FeatureInstructors = await CourseService.GetFeatureInstructors();

        StateHasChanged();
    }


}