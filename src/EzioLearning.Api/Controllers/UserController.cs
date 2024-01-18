using EzioLearning.Core.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EzioLearning.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(IUserRepository userRepository) : ControllerBase
    {

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await userRepository.GetPage(null);
            return Ok(users);
        }
    }
}
