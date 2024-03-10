using AutoMapper;
using EzioLearning.Domain.Entities.Identity;

namespace EzioLearning.Share.Dto.Account
{
    public class AccountInfoDto
    {
        public string? UserName { get; init; }
        public string? FirstName { get; init; }
        public string? FullName { get; init; }
        public string? LastName { get; init; }
        public string? Email { get; init; }
        public string? PhoneNumber { get; init; }

        public DateOnly DateOfBirth { get; init; } = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-10));

        public string[] Roles { get; set; } = [];

        public string? Avatar { get; init; }

        public class AccountInfoDtoProfile : Profile
        {
            public AccountInfoDtoProfile()
            {
                CreateMap<AppUser, AccountInfoDto>();
            }
        }
    }
}
