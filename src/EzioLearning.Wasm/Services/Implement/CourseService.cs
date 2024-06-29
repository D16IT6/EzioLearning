using System.Net.Http.Json;
using Blazored.LocalStorage;
using EzioLearning.Share.Dto.Learning.Course;
using EzioLearning.Share.Dto.Learning.CourseCategory;
using EzioLearning.Share.Dto.User;
using EzioLearning.Share.Models.Pages;
using EzioLearning.Share.Models.Request;
using EzioLearning.Share.Models.Response;
using EzioLearning.Wasm.Dto.Learning.Course;
using EzioLearning.Wasm.Services.Interface;
using EzioLearning.Wasm.Utils.Common;
using EzioLearning.Wasm.Utils.Extensions;

namespace EzioLearning.Wasm.Services.Implement
{
    public class CourseService(HttpClient httpClient, ILocalStorageService localStorageService) : ICourseService
    {

        public async Task AddCourseToFavoriteList(Guid courseId)
        {
            var courseFavoriteList = await localStorageService.GetItemAsync<List<Guid>>(LocalStorageConstants.WishList) ?? [];

            if (!courseFavoriteList.Contains(courseId))
            {
                courseFavoriteList.Add(courseId);
            }
            await localStorageService.SetItemAsync(LocalStorageConstants.WishList, courseFavoriteList);
        }

        public async Task<ResponseBaseWithData<CoursePaymentResponse>> BuyCourse(CoursePaymentRequestDto request)
        {
            var content = new MultipartFormDataContent().CreateFormContent(request);

            var response = await httpClient.PostAsync("api/Payment/Course/VnPay", content);

            return await response.GetResponse<ResponseBaseWithData<CoursePaymentResponse>>();
        }

        public async Task<IEnumerable<CourseViewDto>> GetFeatureCourses(int take = 6)
        {
            var data = new List<CourseViewDto>();
            var response = await httpClient.GetFromJsonAsync<ResponseBaseWithList<CourseViewDto>>(
                $"api/Course/Feature/{take}", JsonCommonOptions.DefaultSerializer);

            if (response is not { IsSuccess: true } || !response.Data.Any()) return data;


            data = response.Data.ToList();

            foreach (var item in data)
            {
                item.TeacherAvatar = ApiConstants.BaseUrl + item.TeacherAvatar;
                item.Poster = ApiConstants.BaseUrl + item.Poster;
            }

            return data;
        }

        public async Task<IEnumerable<TopCourseCategoryDto>> GetTopCourseCategories(int count)
        {
            var data = new List<TopCourseCategoryDto>();
            var response =
                await httpClient.GetFromJsonAsync<ResponseBaseWithList<TopCourseCategoryDto>>(
                    $"api/CourseCategory/Top/{count}", JsonCommonOptions.DefaultSerializer);
            if (response is not { IsSuccess: true } || !response.Data.Any()) return data;
            data = response.Data.ToList();

            foreach (var item in data) item.Image = ApiConstants.BaseUrl + item.Image;

            return data;
        }

        public async Task<int> GetCourseCount()
        {
            var response =
                await httpClient.GetFromJsonAsync<ResponseBaseWithData<int>>("api/Course/Count",
                    JsonCommonOptions.DefaultSerializer);

            var count = response!.Data;


            return GetLargestPart(count);
        }

        public async Task<IEnumerable<InstructorViewDto>> GetFeatureInstructors(int take = 6)
        {
            var data = new List<InstructorViewDto>();
            var response = await httpClient.GetFromJsonAsync<ResponseBaseWithList<InstructorViewDto>>(
                $"api/Course/FeaturedInstructor/{take}", JsonCommonOptions.DefaultSerializer);
            if (response is { IsSuccess: true } && response.Data.Any())
            {
                data = response.Data.ToList();

                foreach (var item in data) item.Avatar = ApiConstants.BaseUrl + item.Avatar;
            }

            return data;
        }

        public async Task<ResponseBaseWithData<CourseCreateDto>> CreateNewCourse(CourseCreateDto courseCreateDto)
        {
            var content = new MultipartFormDataContent().CreateFormContent(courseCreateDto, nameOfFileContent: ["Poster"]);

            var response = await httpClient.PostAsync("api/Course", content);

            return await response.GetResponse<ResponseBaseWithData<CourseCreateDto>>();
        }

        public async Task<ResponseBaseWithData<CourseSectionCreateBlazorDto>> CreateCourseSection(
            CourseSectionCreateBlazorDto courseSectionCreateDto)
        {
            var content = new MultipartFormDataContent().CreateFormContent(courseSectionCreateDto, nameOfFileContent: ["", "File"], excludeProperties: [nameof(CourseLectureCreateBlazorDto.TempFileUrl)]);


            var response = await httpClient.PostAsync("/api/Course/Section", content);

            return await response.GetResponse<ResponseBaseWithData<CourseSectionCreateBlazorDto>>();
        }

        public async Task<ResponseBaseWithData<PageResult<CourseItemViewDto>>> GetCoursePage(CourseListOptions options)
        {
            var queryBuilder = options.CreateQueryString();

            var response =
                await httpClient.GetFromJsonAsync<ResponseBaseWithData<PageResult<CourseItemViewDto>>>(
                    $"api/Course/?{queryBuilder}");

            foreach (var courseInGridViewDto in response!.Data!.Data)
            {
                if (courseInGridViewDto.TeacherAvatar != null)
                    courseInGridViewDto.TeacherAvatar =
                        Path.Combine(ApiConstants.BaseUrl, courseInGridViewDto.TeacherAvatar);

                if (courseInGridViewDto.Poster != null)
                    courseInGridViewDto.Poster =
                        Path.Combine(ApiConstants.BaseUrl, courseInGridViewDto.Poster);
            }

            return response;
        }

        public async Task<ResponseBaseWithData<CourseDetailViewDto>> GetCourseDetailPage(Guid courseId)
        {
            var response =
                await httpClient.GetFromJsonAsync<ResponseBaseWithData<CourseDetailViewDto>>(
                    $"api/Course/Detail/{courseId}", JsonCommonOptions.DefaultSerializer);


            var lectures = (from section in response!.Data!.Sections
                            from lecture in section.Lectures
                            where !string.IsNullOrWhiteSpace(lecture.FileUrl)
                            select lecture).ToList();
            foreach (var lecture in lectures)
            {
                lecture.FileUrl = Path.Combine(ApiConstants.BaseUrl, lecture.FileUrl);
            }

            response.Data.TeacherAvatar = Path.Combine(ApiConstants.BaseUrl, response.Data.TeacherAvatar);



            return response!;
        }

        public async Task<ResponseBaseWithData<PageResult<CourseItemViewDto>>> GetCourseByTeacher(Guid teacherId, CourseListOptions options)
        {
            var queryBuilder = options.CreateQueryString();

            var response =
                await httpClient.GetFromJsonAsync<ResponseBaseWithData<PageResult<CourseItemViewDto>>>(
                    $"api/Course/Teacher/{teacherId}?{queryBuilder}");

            foreach (var courseInGridViewDto in response!.Data!.Data)
            {
                if (courseInGridViewDto.TeacherAvatar != null)
                    courseInGridViewDto.TeacherAvatar =
                        Path.Combine(ApiConstants.BaseUrl, courseInGridViewDto.TeacherAvatar);

                if (courseInGridViewDto.Poster != null)
                    courseInGridViewDto.Poster =
                        Path.Combine(ApiConstants.BaseUrl, courseInGridViewDto.Poster);
            }
            return response;
        }

        public async Task<ResponseBaseWithData<CourseUpdateBlazorDto>> GetCourseUpdate(Guid courseId)
        {
            var response =
                await httpClient.GetFromJsonAsync<ResponseBaseWithData<CourseUpdateBlazorDto>>(
                    $"api/Course/Update/{courseId}");

            response!.Data!.Poster = Path.Combine(ApiConstants.BaseUrl, response.Data.Poster!);
            foreach (var lectureUpdateBlazorDto in response.Data.Sections.SelectMany(x => x.Lectures))
            {

                lectureUpdateBlazorDto.FileUrl = Path.Combine(ApiConstants.BaseUrl, lectureUpdateBlazorDto.FileUrl);
            }

            return response;
        }

        public async Task<ResponseBase> UpdateCourse(CourseUpdateBlazorDto course)
        {
            var content = new MultipartFormDataContent().CreateFormContent(
                model: course,
                prefix: "",
                nameOfFileContent: ["NewPoster", "", "File"],
                excludeProperties: []);

            var response = await httpClient.PutAsync("api/Course", content);

            return await response.GetResponse<ResponseBase>();
        }

        static int GetLargestPart(int number)
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
    }
}
