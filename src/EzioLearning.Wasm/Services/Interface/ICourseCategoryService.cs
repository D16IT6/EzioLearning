using EzioLearning.Share.Dto.Learning.CourseCategory;

namespace EzioLearning.Wasm.Services.Interface
{
	public interface ICourseCategoryService : IServiceBase
	{
		Task<List<CourseCategoryViewDto>> GetCourseCategories();
	}
}
