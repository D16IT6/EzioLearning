using System.Net.Http.Json;
using BlazorAnimate;
using EzioLearning.Blazor.Client.Providers;
using EzioLearning.Core.Dtos.Learning.Course;
using EzioLearning.Core.Dtos.Learning.CourseCategory;
using EzioLearning.Core.Dtos.User;
using EzioLearning.Core.Models.Response;
using EzioLearning.Wasm.Providers;
using Microsoft.AspNetCore.Components;

namespace EzioLearning.Wasm.Pages
{
    public partial class Home
    {

        [Inject] private HttpClient HttpClient { get; set; } = default!;

        private List<CourseCategoryViewDto> CourseCategories { get; set; } = new();
        private int CourseCount { get; set; }

        private List<TopCourseCategoryDto> TopCourseCategories { get; set; } = new();

        private List<CourseViewDto> FeatureCourses { get; set; } = new();
        private List<CourseViewDto> TrendingCourses { get; set; } = new();
        private List<InstructorViewDto> FeatureInstructors { get; set; } = new();

        [Inject] private ILogger<Index> Logger { get; set; } = default!;
        private IAnimation? AnimationType { get; set; }
        private TimeSpan AnimationDuration { get; set; }


        protected override void OnInitialized()
        {
            AnimationType = Animations.FadeUp;
            AnimationDuration = TimeSpan.FromSeconds(1);
        }


        protected override async Task OnInitializedAsync()
        {
            CourseCategories = await GetCourseCategories();

            CourseCount = await GetCourseCount();

            TopCourseCategories = await GetTopCourseCategories(12);

            FeatureCourses = await GetFeatureCourses(6);

            TrendingCourses = await GetFeatureCourses(6);

            FeatureInstructors = await GetFeatureInstructors(6);



        }

        private async Task<List<InstructorViewDto>> GetFeatureInstructors(int take = 6)
        {
            var data = new List<InstructorViewDto>();
            var response = await HttpClient.GetFromJsonAsync<ResponseBaseWithList<InstructorViewDto>>(
                $"api/User/FeaturedInstructor/{take}", JsonCommonOptions.DefaultSerializer);
            if (response is { IsSuccess: true } && response.Data!.Any())
            {
                data = response.Data!;

                foreach (var item in data)
                {
                    item.Avatar = ApiConstants.BaseUrl + item.Avatar;
                }
            }
            return data;

        }

        private async Task<List<CourseViewDto>> GetFeatureCourses(int take = 6)
        {
            var data = new List<CourseViewDto>();
            var response = await HttpClient.GetFromJsonAsync<ResponseBaseWithList<CourseViewDto>>(
                $"api/Course/Feature/{take}", JsonCommonOptions.DefaultSerializer);
            if (response is { IsSuccess: true } && response.Data!.Any())
            {
                data = response.Data!;

                foreach (var item in data)
                {
                    item.TeacherAvatar = ApiConstants.BaseUrl + item.TeacherAvatar;
                    item.Poster = ApiConstants.BaseUrl + item.Poster;
                }
            }
            return data;
        }

        private async Task<List<TopCourseCategoryDto>> GetTopCourseCategories(int count)
        {
            var data = new List<TopCourseCategoryDto>();
            var response = await HttpClient.GetFromJsonAsync<ResponseBaseWithList<TopCourseCategoryDto>>($"api/CourseCategory/Top/{count}", JsonCommonOptions.DefaultSerializer);
            if (response is { IsSuccess: true } && response.Data!.Any())
            {
                data = response.Data!;

                foreach (var item in data)
                {
                    item.Image = ApiConstants.BaseUrl + item.Image;
                }
            }
            return data;

        }

        private async Task<int> GetCourseCount()
        {
            var response = await HttpClient.GetFromJsonAsync<ResponseBaseWithData<int>>("api/Course/Count", JsonCommonOptions.DefaultSerializer);

            var response2 = await HttpClient.GetFromJsonAsync<ResponseBaseWithData<int>>("api/Course/Count", JsonCommonOptions.DefaultSerializer);
            int count = response!.Data;


            return GetLargestPart(count);
        }
        public static int GetLargestPart(int number)
        {
            var numDigits = (int)Math.Floor(Math.Log10(number)) + 1;
            var largestPart = 0;

            for (var i = numDigits - 1; i >= 0; i--)
            {
                var digit = number % 10;
                largestPart += digit * (int)Math.Pow(10, i);
                number /= 10;
            }

            return largestPart;
        }
        private async Task<List<CourseCategoryViewDto>> GetCourseCategories()
        {
            var data = new List<CourseCategoryViewDto>();
            var response =
                await HttpClient.GetFromJsonAsync<ResponseBaseWithList<CourseCategoryViewDto>>("api/CourseCategory", JsonCommonOptions.DefaultSerializer);

            if (response is { IsSuccess: true } && response.Data!.Any())
            {
                data = response.Data!
                    //.Where(x => x.ParentId == Guid.Empty)
                    .ToList();
            }

            return data;
        }


    }
}
