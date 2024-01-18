using AutoMapper;
using EzioLearning.Core.Repositories;
using EzioLearning.Domain.DTO;
using EzioLearning.Domain.Entities.Identity;
using EzioLearning.Infrastructure.DbContext;
using EzioLearning.Infrastructure.SeedWorks;

namespace EzioLearning.Infrastructure.Repositories
{
    public class UserRepository(EzioLearningDbContext context, IMapper mapper) : PagedRepositoryBase<AppUser, Guid, UserDto>(context, mapper), IUserRepository;
}
