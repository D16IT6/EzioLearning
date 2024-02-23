
using AutoMapper;

namespace EzioLearning.Core.Dtos.Learning.CourseCategory
{
	public class CourseCategoryViewDto
	{
		public string? Name { get; set; }
		public Guid? ParentId { get; set; }
		public string? ParentName { get; set; }

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
}
