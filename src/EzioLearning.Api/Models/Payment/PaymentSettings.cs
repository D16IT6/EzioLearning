namespace EzioLearning.Api.Models.Payment
{
    public class PaymentSettings
    {
        public PaypalSettings Paypal { get; set; } = new();

        public PayosSettings Payos { get; set; } = new();
        public class PaypalSettings
        {
            public string ClientId { get; set; } = string.Empty;
            public string ClientSecret { get; set; } = string.Empty;
            public bool IsDevelop { get; set; }
        }
        public class PayosSettings
        {
            public string ClientId { get; set; } = string.Empty;
            public string ApiKey { get; set; } = string.Empty;
            public string ChecksumKey { get; set; } = string.Empty;
        }
    }
}
