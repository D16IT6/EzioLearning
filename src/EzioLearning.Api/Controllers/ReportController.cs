using System.Security.Claims;
using EzioLearning.Core.Repositories.Learning;
using EzioLearning.Share.Common;
using EzioLearning.Share.Dto.Learning.Course;
using EzioLearning.Share.Models.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EzioLearning.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Permissions.Dashboard.View)]
    public class ReportController(
        IStudentRepository studentRepository
    ) : ControllerBase
    {
        [HttpGet("MonthlyIncome")]
        public async Task<IActionResult> GetMonthlyIncome([FromQuery] int year)
        {
            _ = Guid.TryParse(User.Claims.First(x => x.Type.Equals(ClaimTypes.PrimarySid)).Value,out var userId);

            var responseData = await studentRepository.GetMonthlyIncome(year,userId);

            return Ok(new ResponseBaseWithList<MonthlyRevenueItem>()
            {
                Data = responseData.ToList(),
                Message = "Lấy dữ liệu báo cáo thành công"
            });
        }
    }
}
