using EzioLearning.Share.Dto.Learning.Course;
using EzioLearning.Share.Dto.Learning.CourseCategory;
using EzioLearning.Share.Dto.User;
using EzioLearning.Share.Models.Response;
using EzioLearning.Wasm.Dto.Learning.Course;

namespace EzioLearning.Wasm.Services.Interface
{
	public interface ICourseService :IServiceBase
	{
		Task<IEnumerable<CourseViewDto>> GetFeatureCourses(int take = 6);
		Task<IEnumerable<TopCourseCategoryDto>> GetTopCourseCategories(int count);
		Task<int> GetCourseCount();
		Task<IEnumerable<InstructorViewDto>> GetFeatureInstructors(int take = 6);

		Task<ResponseBaseWithData<CourseCreateDto>> CreateNewCourse(CourseCreateDto courseCreateDto);

		Task<ResponseBaseWithData<CourseSectionCreateBlazorDto>> CreateCourseSection(
			CourseSectionCreateBlazorDto courseSectionCreateDto);
	}
}
