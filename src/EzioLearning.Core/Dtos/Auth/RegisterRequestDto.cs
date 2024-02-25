using AutoMapper;
using EzioLearning.Domain.Entities.Identity;
using Microsoft.AspNetCore.Http;

namespace EzioLearning.Core.Dtos.Auth
{
    public class RegisterRequestDto
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? UserName { get; set; }

        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? ConfirmPassword { get; set; }
        public string? PhoneNumber { get; set; }

        public DateOnly DateOfBirth { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-10));

        public IFormFile? Avatar { get; set; }

        public string? LoginProvider { get; set; }
        public string? ProviderName { get; set; }
        public string? ProviderKey { get; set; }


        public class RegisterRequestDtoProfile : Profile
        {
            public RegisterRequestDtoProfile()
            {
                CreateMap<RegisterRequestDto, AppUser>()
                    .ForMember(x => x.Avatar, cfg => cfg.Ignore());
                CreateMap<AppUser, RegisterRequestDto>()
                    .ForMember(x => x.Avatar, cfg => cfg.Ignore()); 
            }
        }
    }
}
