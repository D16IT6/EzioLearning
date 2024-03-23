using AutoMapper;
using EzioLearning.Share.Utils;
using Microsoft.AspNetCore.Http;

namespace EzioLearning.Core.Dto.Learning.Course
{
    public class CourseCreateApiDto
    {
        public CourseStatus Status = CourseStatus.Upcoming;
        public required string Name { get; set; }
        public required string Description { get; set; }
        public IFormFile? Poster { get; set; }

        public string? Content { get; set; }

        public required double Price { get; set; } = 0;
        public required double PromotionPrice { get; set; }
        public int SortOrder { get; set; }
        public CourseLevel Level { get; set; }

        public Guid[] CourseCategoryIds { get; set; } = [];

        public Guid CreatedBy { get; set; }

        
    }
}
