using AutoMapper;
using EzioLearning.Domain.Entities.Identity;

namespace EzioLearning.Share.Dto.Account
{
    public class AccountInfoMinimalDto
    {
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Avatar { get; set; }

        public class AccountInfoMinimalDtoProfile : Profile
        {
            public AccountInfoMinimalDtoProfile()
            {
                CreateMap<AppUser, AccountInfoMinimalDto>();
            }
        }
    }
}
