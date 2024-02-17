
using System.ComponentModel.DataAnnotations;
using AutoMapper;

namespace EzioLearning.Core.Dtos.Learning.CourseCategory
{
    public class CourseCategoryCreateDto
    {

        [Display(Name = "Tên danh mục")]
        [Required(ErrorMessage = "{0} là bắt buộc")]
        [StringLength(50, ErrorMessage = "{0} chỉ dài từ {1} tới {2} ký tự.", MinimumLength = 5)]
        public required string Name { get; set; }
        public bool IsActive { get; set; }
        public Guid? ParentId { get; set; }

        public class CourseCategoryCreateDtoProfile : Profile
        {
            public CourseCategoryCreateDtoProfile()
            {
                CreateMap<CourseCategoryCreateDto, Domain.Entities.Learning.CourseCategory>();
                CreateMap<Domain.Entities.Learning.CourseCategory, CourseCategoryCreateDto>();
            }
        }

    }
}
