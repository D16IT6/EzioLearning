
using AutoMapper;

namespace EzioLearning.Core.Dtos.Learning.Course
{
    public class CourseViewDto
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Poster { get; set; }
        public double Price { get; set; }
        public double PromotionPrice { get; set; }
        public int Lessons { get; set; }
        public double Rating { get; set; }
        public int RateCount { get; set; }
        public Guid CreatedBy { get; set; }

        public string? TeacherName { get; set; }
        public string? TeacherAvatar { get; set; }

        public class CourseViewDtoProfile : Profile
        {
            public CourseViewDtoProfile()
            {
                CreateMap<Domain.Entities.Learning.Course, CourseViewDto>()
                    .ForMember(x => x.Lessons,
                        otp
                            => otp.MapFrom(x => x.Lessons.Count()
                            )
                    );
            }
        }
    }
}
