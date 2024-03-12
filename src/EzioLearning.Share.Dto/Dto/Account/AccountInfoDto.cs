using AutoMapper;
using EzioLearning.Domain.Entities.Identity;

namespace EzioLearning.Share.Dto.Account
{
    public class AccountInfoDto
    {
        public string? UserName { get; set; }
        public string? FirstName { get; set; }
        public string? FullName => FirstName + " " + LastName;
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }

        public DateOnly DateOfBirth { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-10));

        public string[] Roles { get; set; } = [];

        public string? Avatar { get; set; }

        public class AccountInfoDtoProfile : Profile
        {
            public AccountInfoDtoProfile()
            {
                CreateMap<AppUser, AccountInfoDto>().ForMember(x => x.FullName, opt => opt.Ignore());
            }
        }
    }
}
