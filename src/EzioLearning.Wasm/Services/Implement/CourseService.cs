using System.Net.Http.Json;
using EzioLearning.Share.Dto.Learning.Course;
using EzioLearning.Share.Dto.Learning.CourseCategory;
using EzioLearning.Share.Dto.User;
using EzioLearning.Share.Models.Response;
using EzioLearning.Wasm.Services.Interface;
using EzioLearning.Wasm.Utils.Common;

namespace EzioLearning.Wasm.Services.Implement
{
	public class CourseService(HttpClient httpClient) :ICourseService
	{
		public async Task<List<CourseViewDto>> GetFeatureCourses(int take = 6)
		{
			var data = new List<CourseViewDto>();
			var response = await httpClient.GetFromJsonAsync<ResponseBaseWithList<CourseViewDto>>(
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

		public async Task<List<TopCourseCategoryDto>> GetTopCourseCategories(int count)
		{
			var data = new List<TopCourseCategoryDto>();
			var response =
				await httpClient.GetFromJsonAsync<ResponseBaseWithList<TopCourseCategoryDto>>(
					$"api/CourseCategory/Top/{count}", JsonCommonOptions.DefaultSerializer);
			if (response is { IsSuccess: true } && response.Data!.Any())
			{
				data = response.Data!;

				foreach (var item in data) item.Image = ApiConstants.BaseUrl + item.Image;
			}

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
		
		public async Task<List<InstructorViewDto>> GetFeatureInstructors(int take = 6)
		{
			var data = new List<InstructorViewDto>();
			var response = await httpClient.GetFromJsonAsync<ResponseBaseWithList<InstructorViewDto>>(
				$"api/Course/FeaturedInstructor/{take}", JsonCommonOptions.DefaultSerializer);
			if (response is { IsSuccess: true } && response.Data!.Any())
			{
				data = response.Data!;

				foreach (var item in data) item.Avatar = ApiConstants.BaseUrl + item.Avatar;
			}

			return data;
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
