namespace EzioLearning.Api.Models.Payment
{
    public class PayOsWebhook
    {

        public string Code { get; set; } = string.Empty;
        public string Id { get; set; } = string.Empty;
        public bool Cancel { get; set; }

        public string Status { get; set; } = string.Empty;

        public long OrderCode { get; set; }
    }
}
