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


    private List<CourseCategoryViewDto> CourseCategories { get; set; } = new();
    private int CourseCount { get; set; }

    private List<TopCourseCategoryDto> TopCourseCategories { get; set; } = new();

    private List<CourseViewDto> FeatureCourses { get; set; } = new();
    private List<CourseViewDto> TrendingCourses { get; set; } = new();
    private List<InstructorViewDto> FeatureInstructors { get; set; } = new();
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

    }


}