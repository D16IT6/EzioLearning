using EzioLearning.Api.Models.Payment;
using EzioLearning.Api.Services;
using EzioLearning.Api.Services.Vnpay;
using EzioLearning.Domain.Entities.Learning;
using EzioLearning.Share.Dto.Learning.Course;
using EzioLearning.Share.Models.Response;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Net.payOS;
using Net.payOS.Types;
using System.Net;
using System.Security.Claims;
using EzioLearning.Api.Filters;
using EzioLearning.Core.Repositories.Learning;
using EzioLearning.Core.SeedWorks;
using EzioLearning.Domain.Entities.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace EzioLearning.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController(
        PaypalClient paypalClient,
        PayOS payOs,
        ICourseRepository courseRepository,
        IStudentRepository studentRepository,
        IUnitOfWork unitOfWork,
        UserManager<AppUser> userManager,
        VnPayService vnPayService,
        CacheService cacheService
        ) : ControllerBase
    {
        [HttpPost("Course/VnPay")]
        [VerifyToken]
        [Authorize]
        public async Task<IActionResult> BuyCourse([FromForm] CoursePaymentRequestDto model)
        {
            var userIdFromToken = Guid.Parse(User.Claims.First(x => x.Type == ClaimTypes.PrimarySid).Value);
            if (model.UserId != userIdFromToken)
                return BadRequest(new ResponseBase()
                {
                    Message = "Đừng hack server",
                    Status = HttpStatusCode.BadRequest
                });

            var user = userManager.Users.Include(x=> x.Courses).FirstOrDefault(x => x.Id == model.UserId);
            var course = (await courseRepository.Find(x => x.Id == model.CourseId)).FirstOrDefault();

            var student = (await studentRepository.Find(x => x.CourseId == model.CourseId && x.UserId == model.UserId && x.Confirm))
                .AsQueryable().FirstOrDefault();
            if (user == null || course == null || user.Courses.Contains(course) || student != null)
                return NotFound(new ResponseBase()
                {
                    Status = HttpStatusCode.NotFound,
                    Message = "Thông tin gửi lỗi hoặc bạn đã mua rồi!"
                });
            var oldStudents = (await studentRepository.Find(x => x.CourseId == model.CourseId && x.UserId == model.UserId && !x.Confirm))
                .AsQueryable();
            if (oldStudents.Any())
            {
                studentRepository.RemoveRange(oldStudents);
                await unitOfWork.CompleteAsync();
            }
            var newOrder = new Student()
            {
                Id = Guid.NewGuid(),
                CourseId = model.CourseId,
                UserId = user.Id,
                Price = course.Price,
                Confirm = true
            };

            cacheService.Set(newOrder.Id.ToString(), newOrder,TimeSpan.FromMinutes(15));

            var paymentUrl = vnPayService.CreatePaymentUrl(ControllerContext.HttpContext, new VnPaymentRequestModel()
            {
                Description = $"Mua khoá học {course.Name}",
                Amount = model.Price,
                CreatedDate = DateTime.UtcNow,
                OrderId = newOrder.Id
            });
            
            return Ok(new ResponseBaseWithData<CoursePaymentResponse>()
            {
                Status = HttpStatusCode.OK,
                Data = new CoursePaymentResponse()
                {
                    OrderId = newOrder.Id,
                    Url = paymentUrl
                }
            });
        }

        [HttpGet("Course/VnPay/Callback")]
        public async Task<IActionResult> BuyCourseVnPayCallback()
        {
            var info = vnPayService.PaymentExecute(ControllerContext.HttpContext.Request.Query);
            //var order = await (await studentRepository.Find(x => x.Id == info.OrderId)).AsQueryable().FirstAsync();

            if (info.Success)
            {
                var cacheData = cacheService.Get<Student>(info.OrderId.ToString());
                if(cacheData != null)
                {

                    var user = userManager.Users.First(x => x.Id == cacheData.UserId);
                    var course = (await courseRepository.Find(x => x.Id == cacheData.CourseId)).AsQueryable().First();

                    cacheData.Course = course;
                    cacheData.User = user;
                    cacheData.Confirm = true;

                    course.Students.Add(cacheData);
                    studentRepository.Add(cacheData);

                    await unitOfWork.CompleteAsync();
                }
            }

            var url = "https://localhost:17233/Checkout";
            return Redirect($"{url}?Success={info.Success}&orderId={info.OrderId}");
        }

        [HttpPost("Paypal")]
        public async Task<IActionResult> PaypalCreate(double price = 123)
        {
            var order = await paypalClient.CreateOrder(price, PaymentCurrency.Usd, Guid.NewGuid().ToString());

            return Ok(order);
        }


        [HttpGet("Paypal/Capture")]
        public async Task<IActionResult> PaypalCapture(string orderId)
        {
            var response = await paypalClient.CaptureOrder(orderId);

            //Save to db
            return Ok(new ResponseBaseWithData<CaptureOrderResponse>()
            {
                Data = response,
                Message = "Lưu hoá đơn thành công"
            });
        }

        [HttpGet("Payos")]
        public async Task<IActionResult> PayosCreate()
        {
            var orderCode = int.Parse(DateTimeOffset.Now.ToString("ffffff"));
            var item = new ItemData("C# pro max", 1, 1000);
            var item2 = new ItemData("LinQ # pro max", 1, 1000);
            var items = new List<ItemData> { item,item2 };

            var amount = items.Sum(x => x.price * x.quantity);
            var paymentData = new PaymentData(orderCode, amount, "ezio learning", items,
                "https://localhost:7000/api/Payment/PayOs/Webhook",
                "https://localhost:7000/api/Payment/PayOs/Webhook"
                );

            CreatePaymentResult createPayment = await payOs.createPaymentLink(paymentData);

            return Ok(new ResponseBaseWithData<CreatePaymentResult>()
            {
                Data = createPayment
            });
        }

        [HttpGet("Payos/Webhook")]
        public async Task<IActionResult> PayOsWebhook([FromQuery] PayOsWebhook model)
        {
            PaymentLinkInformation paymentLinkInformation;
            if (!model.Cancel)
            {
                paymentLinkInformation = await payOs.getPaymentLinkInfomation(model.OrderCode);
            }
            else
            {
                paymentLinkInformation = await payOs.cancelPaymentLink(model.OrderCode);
            }
           
            return Ok(new ResponseBaseWithData<PaymentLinkInformation>()
            {
                Data = paymentLinkInformation
            });
        }
        [HttpGet("Payos/Cancel")]
        public async Task<IActionResult> PayOsCancel([FromQuery] PayOsWebhook model)
        {
            PaymentLinkInformation paymentLinkInformation = await payOs.cancelPaymentLink(model.OrderCode);

            return Ok(new ResponseBaseWithData<PaymentLinkInformation>()
            {
                Data = paymentLinkInformation
            });
        }
        [HttpGet("Payos/Success")]
        public async Task<IActionResult> PayOsSuccess([FromQuery] PayOsWebhook model)
        {
            PaymentLinkInformation paymentLinkInformation = await payOs.getPaymentLinkInfomation(model.OrderCode);

            return Ok(new ResponseBaseWithData<PaymentLinkInformation>()
            {
                Data = paymentLinkInformation
            });
        }
    }
}
