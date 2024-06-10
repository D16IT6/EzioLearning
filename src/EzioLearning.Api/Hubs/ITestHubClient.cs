using EzioLearning.Share.Dto.User;

namespace EzioLearning.Api.Hubs
{
    public interface ITestHubClient
    {
        Task ReceiveUsers(IEnumerable<UserDto> userDtos);
    }
}
