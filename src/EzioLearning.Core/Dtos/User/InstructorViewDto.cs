using AutoMapper;
using EzioLearning.Domain.Entities.Identity;

namespace EzioLearning.Core.Dtos.User
{
    public class InstructorViewDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = String.Empty;
        public int StudentCount { get; set; }
        public string Avatar { get; set; } = String.Empty;

        public class InstructorViewDtoProfile : Profile
        {
            public InstructorViewDtoProfile()
            {
                CreateMap<AppUser, InstructorViewDto>()
                    .ForMember(x => x.Name, cfg => cfg.MapFrom(x => $"{x.FirstName} {x.LastName}"))
                    .ForMember(x => x.StudentCount, cfg => cfg.MapFrom(x => x.Students.Count));
            }
        }
    }
}
