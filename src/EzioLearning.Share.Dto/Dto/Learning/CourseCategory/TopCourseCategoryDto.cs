using AutoMapper;

namespace EzioLearning.Share.Dto.Learning.CourseCategory;

public class TopCourseCategoryDto
{
    public string Name { get; init; } = string.Empty;
    public string? Image { get; set; } = string.Empty;

    public class TopCourseCategoryDtoProfile : Profile
    {
        public TopCourseCategoryDtoProfile()
        {
            CreateMap<TopCourseCategoryDtoProfile, Domain.Entities.Learning.CourseCategory>();
            CreateMap<Domain.Entities.Learning.CourseCategory, TopCourseCategoryDtoProfile>();
        }
    }
}