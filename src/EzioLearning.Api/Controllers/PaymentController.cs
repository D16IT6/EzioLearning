using EzioLearning.Api.Models.Payment;
using EzioLearning.Api.Services;
using EzioLearning.Share.Models.Response;
using Microsoft.AspNetCore.Mvc;

namespace EzioLearning.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController(PaypalClient paypalClient) : ControllerBase
    {

        [HttpGet("Paypal")]
        public async Task<IActionResult> PaypalCreateOrder([FromQuery] string orderId)
        {
            return Ok(await paypalClient.CaptureOrder(orderId));
        }

        [HttpPut("Paypal")]
        public async Task<IActionResult> PaypalCreate(double price = 123)
        {
            var order = await paypalClient.CreateOrder(price, PaymentCurrency.Usd, Guid.NewGuid().ToString());
            await paypalClient.CaptureOrder(order.Id);

            return Ok(new ResponseBaseWithData<CreateOrderResponse>()
            {
                Data = order,
                Message = "Tạo hoá đơn thành công"
            });
        }

        //[HttpPost("Paypal/Callback")]
        //public async Task<IActionResult> PaypalCallback(CaptureOrderResponse webhookData)
        //{
        //    return Ok(webhookData);
        //} 
        
        //[HttpPost("Paypal/Cancel")]
        //public async Task<IActionResult> PaypalCancel(CaptureOrderResponse webhookData)
        //{
        //    return Ok(webhookData);
        //}

    }
}
