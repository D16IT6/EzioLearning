using EzioLearning.Share.Dto.Learning.Course;
using EzioLearning.Share.Dto.Learning.CourseCategory;
using EzioLearning.Share.Dto.User;
using EzioLearning.Share.Models.Pages;
using EzioLearning.Share.Models.Request;
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

		Task<ResponseBaseWithData<PageResult<CourseItemViewDto>>> GetCoursePage(CourseListOptions options);

		Task<ResponseBaseWithData<CourseDetailViewDto>> GetCourseDetailPage(Guid courseId);
		Task<ResponseBaseWithData<PageResult<CourseItemViewDto>>> GetCourseByTeacher(Guid teacherId, CourseListOptions options);
		Task<ResponseBaseWithData<CourseUpdateBlazorDto>> GetCourseUpdate(Guid courseId);
		Task<ResponseBase> UpdateCourse(CourseUpdateBlazorDto course);

		Task AddCourseToFavoriteList(Guid courseId);


		Task<ResponseBaseWithData<CoursePaymentResponse>> BuyCourse(CoursePaymentRequestDto request);
		Task<ResponseBaseWithData<PageResult<CoursePurchasedItemViewDto>>> GetPurchasedCourses(CourseListOptions courseListOptions);
	}
}
