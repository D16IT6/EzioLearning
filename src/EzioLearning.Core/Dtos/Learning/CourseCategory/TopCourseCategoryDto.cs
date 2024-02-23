
using AutoMapper;

namespace EzioLearning.Core.Dtos.Learning.CourseCategory
{
    public class TopCourseCategoryDto
    {
        public string Name { get; set; } = String.Empty;
        public string? Image { get; set; } = String.Empty;

        public class TopCourseCategoryDtoProfile : Profile
        {
            public TopCourseCategoryDtoProfile()
            {
                CreateMap<TopCourseCategoryDtoProfile, Domain.Entities.Learning.CourseCategory>();
                CreateMap<Domain.Entities.Learning.CourseCategory, TopCourseCategoryDtoProfile>();
            }
        }
    }
}
