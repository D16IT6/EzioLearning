using System.Globalization;

namespace EzioLearning.Api.Services.Vnpay
{
    public class VnPayService(VnPayClient vnpay)
    {
        public string CreatePaymentUrl(HttpContext context, VnPaymentRequestModel model)
        {

            vnpay.AddRequestData("vnp_Version", vnpay.Settings.Version!);
            vnpay.AddRequestData("vnp_Command", vnpay.Settings.Command!);
            vnpay.AddRequestData("vnp_TmnCode", vnpay.Settings.TmnCode!);
            vnpay.AddRequestData("vnp_Amount", (model.Amount * 100).ToString(CultureInfo.InvariantCulture));

            vnpay.AddRequestData("vnp_CreateDate", model.CreatedDate.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", vnpay.Settings.CurrCode!);
            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress(context));
            vnpay.AddRequestData("vnp_Locale", vnpay.Settings.Locale!);

            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toán cho đơn hàng:" + model.OrderId);
            vnpay.AddRequestData("vnp_OrderType", "other"); //default value: other
            vnpay.AddRequestData("vnp_ReturnUrl", vnpay.Settings.ReturnUrl!);

            vnpay.AddRequestData("vnp_TxnRef", model.OrderId.ToString()); 

            var paymentUrl = vnpay.CreateRequestUrl(vnpay.Settings.Url!, vnpay.Settings.HashSecret!);

            return paymentUrl;
        }

        public VnPaymentResponseModel PaymentExecute(IQueryCollection collections)
        {
            foreach (var (key, value) in collections)
            {
                if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                {
                    vnpay.AddResponseData(key, value.ToString());
                }
            }

            var vnpOrderId = Guid.Parse(vnpay.GetResponseData("vnp_TxnRef"));
            var vnpTransactionId = Convert.ToInt64(vnpay.GetResponseData("vnp_TransactionNo"));
            var vnpSecureHash = collections.FirstOrDefault(p => p.Key == "vnp_SecureHash").Value;
            var vnpResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
            var vnpOrderInfo = vnpay.GetResponseData("vnp_OrderInfo");

            bool checkSignature = vnpay.ValidateSignature(vnpSecureHash!, vnpay.Settings.HashSecret!);
            if (!checkSignature)
            {
                return new VnPaymentResponseModel
                {
                    Success = false
                };
            }

            return new VnPaymentResponseModel
            {
                Success = true,
                PaymentMethod = "VnPay",
                OrderDescription = vnpOrderInfo,
                OrderId = vnpOrderId,
                TransactionId = vnpTransactionId,
                Token = vnpSecureHash,
                VnPayResponseCode = vnpResponseCode
            };
        }
    }
}
