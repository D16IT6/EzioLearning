using System.Net.Http.Json;
using EzioLearning.Share.Dto.Learning.CourseCategory;
using EzioLearning.Share.Models.Response;
using EzioLearning.Wasm.Services.Interface;
using EzioLearning.Wasm.Utils.Common;

namespace EzioLearning.Wasm.Services.Implement
{
	public class CourseCategoryService(HttpClient httpClient) : ICourseCategoryService
	{
		private static readonly List<CourseCategoryViewDto> CourseCategoryViewDtos = new();
		public async Task<List<CourseCategoryViewDto>> GetCourseCategories()
		{
			if (CourseCategoryViewDtos.Any()) return CourseCategoryViewDtos;

			var response =
				await httpClient.GetFromJsonAsync<ResponseBaseWithList<CourseCategoryViewDto>>("api/CourseCategory",
					JsonCommonOptions.DefaultSerializer);

			if (response is { IsSuccess: true } && response.Data!.Any())
			{
				var data = response.Data!
					//.Where(x => x.ParentId == Guid.Empty)
					.ToList();
				CourseCategoryViewDtos.AddRange(data);
            }

            return CourseCategoryViewDtos;
		}
	}
}
