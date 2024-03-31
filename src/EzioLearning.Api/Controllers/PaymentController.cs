using EzioLearning.Api.Models.Payment;
using EzioLearning.Api.Services;
using EzioLearning.Share.Models.Response;
using Microsoft.AspNetCore.Mvc;
using Net.payOS;
using Net.payOS.Types;

namespace EzioLearning.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController(PaypalClient paypalClient, PayOS payOs) : ControllerBase
    {


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
