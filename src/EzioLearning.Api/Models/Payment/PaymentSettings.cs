namespace EzioLearning.Api.Models.Payment
{
    public class PaymentSettings
    {
        public PaypalSettings Paypal { get; set; } = new();

        public class PaypalSettings
        {
            public string ClientId { get; set; } = string.Empty;
            public string ClientSecret { get; set; } = string.Empty;
            public bool IsDevelop { get; set; }
        }
    }
}
