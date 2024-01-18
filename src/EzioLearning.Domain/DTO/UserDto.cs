using AutoMapper;
using EzioLearning.Domain.Entities.Identity;

namespace EzioLearning.Domain.DTO
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string? UserName { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }

        public class AutoMapperProfile : Profile
        {
            public AutoMapperProfile()
            {
                CreateMap<AppUser, UserDto>();
            }
        }
    }
}
