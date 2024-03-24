using EzioLearning.Share.Utils;
using Microsoft.AspNetCore.Components.Forms;

namespace EzioLearning.Share.Dto.Learning.Course;

public class CourseCreateDto
{
    public CourseStatus Status = CourseStatus.Upcoming;
    public required string Name { get; set; }
    public required string Description { get; set; }
    public IBrowserFile? Poster { get; set; }

    public string? Content { get; set; }

    public required double Price { get; set; } = 0;
    public required double PromotionPrice { get; set; }
    public int SortOrder { get; set; }
    public CourseLevel Level { get; set; }

    public Guid[] CourseCategoryIds { get; set; } = [];

    public Guid CreatedBy { get; set; }

    public class CourseCreateDtoProfile : Profile
    {
        public CourseCreateDtoProfile()
        {
            CreateMap<CourseCreateDto, Domain.Entities.Learning.Course>();
            //.ForMember(x => x.Categories, memberOptions: expression =>
            //{
            //    expression.MapFrom(mapExpression: x =>
            //        GetInsertCourseCategories(x)
            //        );
            //});
        }


        //private async Task<IEnumerable<Domain.Entities.Learning.CourseCategory>> GetInsertCourseCategories(
        //    CourseCreateDto courseCreateDto)
        //{
        //    var result = new List<Domain.Entities.Learning.CourseCategory>();
        //    foreach (var courseCategoryId in courseCreateDto.CourseCategoryIds)
        //    {
        //        var insertItem = await _courseCategoryRepository.GetByIdAsync(courseCategoryId);
        //        if (insertItem != null)
        //            result.Add(insertItem);
        //    }
        //    return result;
        //}
    }
}