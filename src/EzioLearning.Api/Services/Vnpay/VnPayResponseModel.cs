namespace EzioLearning.Api.Services.Vnpay
{
    public class VnPaymentResponseModel
    {
        public bool Success { get; set; }
        public string? PaymentMethod { get; set; }
        public string? OrderDescription { get; set; }
        public Guid OrderId { get; set; }
        public string? PaymentId { get; set; }
        public long TransactionId { get; set; }
        public string? Token { get; set; }
        public string? VnPayResponseCode { get; set; }
    }

}
