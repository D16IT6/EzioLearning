using System.Net.Http.Json;
using EzioLearning.Share.Dto.Learning.CourseCategory;
using EzioLearning.Share.Models.Response;
using EzioLearning.Wasm.Services.Interface;
using EzioLearning.Wasm.Utils.Common;

namespace EzioLearning.Wasm.Services.Implement
{
	public class CourseCategoryService(HttpClient httpClient) : ICourseCategoryService
	{
		public async Task<List<CourseCategoryViewDto>> GetCourseCategories()
		{
			var data = new List<CourseCategoryViewDto>();
			var response =
				await httpClient.GetFromJsonAsync<ResponseBaseWithList<CourseCategoryViewDto>>("api/CourseCategory",
					JsonCommonOptions.DefaultSerializer);

			if (response is { IsSuccess: true } && response.Data!.Any())
				data = response.Data!
					//.Where(x => x.ParentId == Guid.Empty)
					.ToList();

			return data;
		}
	}
}
