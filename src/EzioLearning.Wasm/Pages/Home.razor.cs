using BlazorAnimate;
using EzioLearning.Share.Dto.Learning.Course;
using EzioLearning.Share.Dto.Learning.CourseCategory;
using EzioLearning.Share.Dto.User;
using EzioLearning.Share.Models.Response;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;
using EzioLearning.Wasm.Services.Interface;
using EzioLearning.Wasm.Utils.Common;

namespace EzioLearning.Wasm.Pages;

public partial class Home
{
    [Inject] private HttpClient HttpClient { get; set; } = default!;

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
        CourseCategories = await CourseCategoryService.GetCourseCategories();

        CourseCount = await CourseService.GetCourseCount();

        TopCourseCategories = await CourseService.GetTopCourseCategories(12);

        FeatureCourses = await CourseService.GetFeatureCourses();

        TrendingCourses = await CourseService.GetFeatureCourses();

        FeatureInstructors = await CourseService.GetFeatureInstructors();
    }


}