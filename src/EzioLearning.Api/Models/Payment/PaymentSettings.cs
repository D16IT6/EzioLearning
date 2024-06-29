namespace EzioLearning.Api.Models.Payment
{
    public class PaymentSettings
    {
        public PaypalSettings Paypal { get; set; } = new();

        public PayOsSettings PayOs { get; set; } = new();

        public VnPaySettings VnPay { get; set; } = new();
        public class PaypalSettings
        {
            public string ClientId { get; set; } = string.Empty;
            public string ClientSecret { get; set; } = string.Empty;
            public bool IsDevelop { get; set; }
        }
        public class PayOsSettings
        {
            public string ClientId { get; set; } = string.Empty;
            public string ApiKey { get; set; } = string.Empty;
            public string ChecksumKey { get; set; } = string.Empty;
        }

        public class VnPaySettings
        {
            public string? TmnCode { get; set; }
            public string? HashSecret { get; set; }
            public string? Url { get; set; }
            public string? Version { get; set; }
            public string? Command { get; set; }
            public string? CurrCode { get; set; }
            public string? Locale { get; set; }
            public string? ReturnUrl { get; set; }
        }
    }
}
