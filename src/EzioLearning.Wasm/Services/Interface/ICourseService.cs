using EzioLearning.Share.Dto.Learning.Course;
using EzioLearning.Share.Dto.Learning.CourseCategory;
using EzioLearning.Share.Dto.User;

namespace EzioLearning.Wasm.Services.Interface
{
	public interface ICourseService :IServiceBase
	{
		Task<List<CourseViewDto>> GetFeatureCourses(int take = 6);
		Task<List<TopCourseCategoryDto>> GetTopCourseCategories(int count);
		Task<int> GetCourseCount();
		Task<List<InstructorViewDto>> GetFeatureInstructors(int take = 6);
	}
}
