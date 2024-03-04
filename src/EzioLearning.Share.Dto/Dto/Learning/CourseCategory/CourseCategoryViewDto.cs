using AutoMapper;

namespace EzioLearning.Share.Dto.Learning.CourseCategory;

public class CourseCategoryViewDto
{
    public string? Name { get; init; }
    public Guid? ParentId { get; init; }
    public string? ParentName { get; init; }

    public class CourseCategoryViewDtoProfile : Profile
    {
        public CourseCategoryViewDtoProfile()
        {
            CreateMap<CourseCategoryViewDto, Domain.Entities.Learning.CourseCategory>();

            CreateMap<Domain.Entities.Learning.CourseCategory, CourseCategoryViewDto>()
                .ForMember(dest => dest.ParentName,
                    otp =>
                        otp.MapFrom(src => src.Parent!.Name));
        }
    }
}