using Microsoft.AspNetCore.Components.Forms;

namespace EzioLearning.Share.Dto.Learning.CourseCategory;

public class CourseCategoryCreateDto
{
    [Display(Name = "Tên danh mục")]
    [Required(ErrorMessage = "{0} là bắt buộc")]
    [StringLength(50, ErrorMessage = "{0} chỉ dài từ {1} tới {2} ký tự.", MinimumLength = 5)]
    public required string Name { get; init; }

    public IBrowserFile? Image { get; init; }
    public bool IsActive { get; init; }
    public Guid? ParentId { get; init; }

    public class CourseCategoryCreateDtoProfile : Profile
    {
        public CourseCategoryCreateDtoProfile()
        {
            CreateMap<CourseCategoryCreateDto, Domain.Entities.Learning.CourseCategory>();
            CreateMap<Domain.Entities.Learning.CourseCategory, CourseCategoryCreateDto>();
        }
    }
}