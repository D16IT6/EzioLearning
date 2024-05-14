using Microsoft.AspNetCore.Mvc;

namespace EzioLearning.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class _HowToController : ControllerBase
    {
        [HttpGet]
        public Task<IActionResult> Get()
        {
            return Task.FromResult<IActionResult>(Ok(new
            {
                I18n = "Thêm Accept-Language vào header để đổi ngôn ngữ. Hiện tại hỗ trợ :vi-VN, en-US",
                Message="Hiện tại chức năng đăng nhập, đăng ký, thông tin tài khoản có thể sử dụng, các chức năng khác đang phát triển"

            }));
        }
    }
}
