using AutoMapper;
using EzioLearning.Domain.Entities.Identity;

namespace EzioLearning.Share.Dto.User;

public class InstructorViewDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public int StudentCount { get; init; }
    public string Avatar { get; set; } = string.Empty;

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