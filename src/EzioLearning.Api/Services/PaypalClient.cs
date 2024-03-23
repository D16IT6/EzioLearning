using System.Globalization;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using EzioLearning.Api.Models.Payment;

namespace EzioLearning.Api.Services
{
    public sealed class PaypalClient(PaymentSettings.PaypalSettings paypal)
    {
        public string ClientId { get; } = paypal.ClientId;
        public string ClientSecret { get; } = paypal.ClientSecret;

        public string BaseUrl => paypal.IsDevelop
            ? "https://api-m.sandbox.paypal.com"
                : "https://api-m.paypal.com";

        public async Task<AuthResponse> Authenticate()
        {
            var auth = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{ClientId}:{ClientSecret}"));

            var content = new List<KeyValuePair<string, string>>
            {
                new("grant_type", "client_credentials")
            };

            var httpClient = new HttpClient()
            {
                BaseAddress = new Uri(BaseUrl)
            };
            httpClient.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse($"Basic {auth}");

            var httpResponse = await httpClient.PostAsync("/v1/oauth2/token", new FormUrlEncodedContent(content));
            var jsonResponse = await httpResponse.Content.ReadAsStringAsync();

            var response = JsonSerializer.Deserialize<AuthResponse>(jsonResponse);

            httpClient.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse($"Bearer {response!.AccessToken}");

            var responseWebHook = await httpClient.GetAsync("/v1/notifications/webhooks");

            var webHookStream = await responseWebHook.Content.ReadAsStreamAsync();

            var webhookResponse = await JsonSerializer.DeserializeAsync<WebhookResponse>(webHookStream);
            if (webhookResponse!.Webhooks.Any())
            {
                var id = webhookResponse.Webhooks.First().Id;
                var responseWebHookDelete = await httpClient.DeleteAsync($"/v1/notifications/webhooks/{id}");

            }

            var webhookCreate = new WebhookCreate()
            {
                Url = "https://ce59-222-252-29-229.ngrok-free.app/api/Payment/Paypal/Callback",
                EventTypes =
                [
                    new EventType(){Name="*"},
                ]
            };
            var responseWebHook2 = await httpClient.PostAsJsonAsync("v1/notifications/webhooks", webhookCreate);




            return response;
        }

        public async Task<CreateOrderResponse> CreateOrder(double price, string currency, string reference)
        {
            var auth = await Authenticate();

            var request = new CreateOrderRequest
            {
                Intent = "CAPTURE",
                PurchaseUnits =
                [
                    new PurchaseUnit
                    {
                        ReferenceId = reference,
                        Amount = new Amount
                        {
                            CurrencyCode = currency,
                            Value = price.ToString(CultureInfo.InvariantCulture)
                        }
                    }
                ],
                //ApplicationContext = new ApplicationContext()
                //{
                //    //CancelUrl = "https://931e-171-255-79-29.ngrok-free.app/api/Payment/Paypal/Cancel",
                //    //ReturnUrl = "https://931e-171-255-79-29.ngrok-free.app/api/Payment/Paypal/Callback"
                //    ReturnUrl = "http://localhost:7000/api/Payment/Paypal/Callback"
                //}

            };

            var httpClient = new HttpClient()
            {
                BaseAddress = new Uri(BaseUrl)
            };

            httpClient.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse($"Bearer {auth.AccessToken}");


            var httpResponse = await httpClient.PostAsJsonAsync("/v2/checkout/orders", request);

            var jsonResponse = await httpResponse.Content.ReadAsStringAsync();
            var response = JsonSerializer.Deserialize<CreateOrderResponse>(jsonResponse);

            return response!;
        }

        public async Task<CaptureOrderResponse> CaptureOrder(string orderId)
        {
            var auth = await Authenticate();

            var httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse($"Bearer {auth.AccessToken}");

            var httpContent = new StringContent("", Encoding.Default, "application/json");

            var httpResponse = await httpClient.PostAsync($"{BaseUrl}/v2/checkout/orders/{orderId}/capture", httpContent);

            var jsonResponse = await httpResponse.Content.ReadAsStringAsync();
            var response = JsonSerializer.Deserialize<CaptureOrderResponse>(jsonResponse);

            return response!;
        }
    }

    public sealed class AuthResponse
    {
        [JsonPropertyName("scope")] public string Scope { get; set; } = string.Empty;

        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; } = string.Empty;

        [JsonPropertyName("token_type")]
        public string TokenType { get; set; } = string.Empty;

        [JsonPropertyName("app_id")]
        public string AppId { get; set; } = string.Empty;

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonPropertyName("nonce")]
        public string Nonce { get; set; } = string.Empty;
    }
    public sealed class CreateOrderRequest
    {
        [JsonPropertyName("intent")] public string Intent { get; set; } = string.Empty;
        [JsonPropertyName("purchase_units")] public List<PurchaseUnit> PurchaseUnits { get; set; } = new();

        //[JsonPropertyName("application_context")]
        //public ApplicationContext ApplicationContext = new();
        //[JsonPropertyName("payment_source")] public List<PaymentSource> PaymentSources { get; set; } = new();
    }

    public sealed class ApplicationContext
    {
        [JsonPropertyName("return_url")]
        public string ReturnUrl { get; set; } = string.Empty;
        [JsonPropertyName("cancel_url")]
        public string CancelUrl { get; set; } = string.Empty;
    }
    public sealed class CreateOrderResponse
    {
        [JsonPropertyName("id")] public string Id { get; set; } = string.Empty;
        [JsonPropertyName("status")] public string Status { get; set; } = string.Empty;

        [JsonPropertyName("links")] public List<Link> Links { get; set; } = new();
    }
    public sealed class CaptureOrderResponse
    {
        [JsonPropertyName("id")] public string Id { get; set; } = string.Empty;
        [JsonPropertyName("status")] public string Status { get; set; } = string.Empty;

        [JsonPropertyName("payment_source")]
        public PaymentSource PaymentSource { get; set; } = new();
        [JsonPropertyName("purchase_units")] public List<PurchaseUnit> PurchaseUnits { get; set; } = new();

        [JsonPropertyName("payer")]
        public Payer Payer { get; set; } = new();
        [JsonPropertyName("links")] public List<Link> Links { get; set; } = new();
    }
    public sealed class PurchaseUnit
    {
        [JsonPropertyName("amount")] public Amount Amount { get; set; } = new();
        [JsonPropertyName("reference_id")] public string ReferenceId { get; set; } = string.Empty;
        [JsonPropertyName("shipping")] public Shipping Shipping { get; set; } = new();
        [JsonPropertyName("payments")] public Payments Payments { get; set; } = new();
    }
    public sealed class Payments
    {
        [JsonPropertyName("captures")]
        public List<Capture> Captures { get; set; } = [];
    }
    public sealed class Shipping
    {
        [JsonPropertyName("shipping")] public Address Address { get; set; } = new();
    }
    public class Capture
    {
        [JsonPropertyName("id")] public string Id { get; set; } = string.Empty;

        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        [JsonPropertyName("amount")] public Amount Amount { get; set; } = new();

        [JsonPropertyName("seller_protection")]
        public SellerProtection SellerProtection { get; set; } = new();

        [JsonPropertyName("final_capture")]
        public bool FinalCapture { get; set; }

        [JsonPropertyName("disbursement_mode")]
        public string DisbursementMode { get; set; } = string.Empty;

        [JsonPropertyName("seller_receivable_breakdown")]
        public SellerReceivableBreakdown SellerReceivableBreakdown { get; set; } = new();

        [JsonPropertyName("create_time")]
        public DateTime CreateTime { get; set; }

        [JsonPropertyName("update_time")]
        public DateTime UpdateTime { get; set; }

        [JsonPropertyName("links")] public List<Link> Links { get; set; } = [];
    }
    public class Amount
    {
        [JsonPropertyName("currency_code")]
        public string CurrencyCode { get; set; } = string.Empty;

        [JsonPropertyName("value")] public string Value { get; set; } = string.Empty;
    }
    public sealed class Link
    {
        [JsonPropertyName("href")]
        public string Href { get; set; } = string.Empty;

        [JsonPropertyName("rel")]
        public string Rel { get; set; } = string.Empty;

        [JsonPropertyName("method")]
        public string Method { get; set; } = string.Empty;
    }
    public sealed class Name
    {
        [JsonPropertyName("given_name")]
        public string GivenName { get; set; } = string.Empty;

        [JsonPropertyName("surname")]
        public string Surname { get; set; } = string.Empty;
    }
    public sealed class SellerProtection
    {
        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        [JsonPropertyName("dispute_categories")]
        public List<string> DisputeCategories { get; set; } = [];
    }
    public sealed class SellerReceivableBreakdown
    {
        [JsonPropertyName("gross_amount")] public Amount GrossAmount { get; set; } = new();

        [JsonPropertyName("paypal_fee")] public PaypalFee PaypalFee { get; set; } = new();

        [JsonPropertyName("net_amount")] public Amount NetAmount { get; set; } = new();
    }
    public sealed class Paypal
    {
        [JsonPropertyName("name")] public Name Name { get; set; } = new();

        [JsonPropertyName("email_address")]
        public string EmailAddress { get; set; } = string.Empty;

        [JsonPropertyName("account_id")]
        public string AccountId { get; set; } = string.Empty;
    }
    public sealed class PaypalFee
    {
        [JsonPropertyName("currency_code")]
        public string CurrencyCode { get; set; } = string.Empty;

        [JsonPropertyName("value")]
        public string Value { get; set; } = string.Empty;
    }
    public class Address
    {
        [JsonPropertyName("address_line_1")]
        public string AddressLine1 { get; set; } = string.Empty;

        [JsonPropertyName("address_line_2")]
        public string AddressLine2 { get; set; } = string.Empty;

        [JsonPropertyName("admin_area_2")]
        public string AdminArea2 { get; set; } = string.Empty;

        [JsonPropertyName("admin_area_1")]
        public string AdminArea1 { get; set; } = string.Empty;

        [JsonPropertyName("postal_code")]
        public string PostalCode { get; set; } = string.Empty;

        [JsonPropertyName("country_code")]
        public string CountryCode { get; set; } = string.Empty;
    }
    public sealed class Payer
    {
        [JsonPropertyName("name")] public Name Name { get; set; } = new();

        [JsonPropertyName("email_address")]
        public string EmailAddress { get; set; } = string.Empty;

        [JsonPropertyName("payer_id")]
        public string PayerId { get; set; } = string.Empty;
    }
    public sealed class PaymentSource
    {
        [JsonPropertyName("paypal")] public Paypal Paypal { get; set; } = new();
    }


    #region MyRegion
    // Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);
    public class EventType
    {
        [JsonPropertyName("name")] public string Name { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;
    }

    public class WebhookCreate
    {
        [JsonPropertyName("url")] public string Url { get; set; } = string.Empty;

        [JsonPropertyName("event_types")] public List<EventType> EventTypes { get; set; } = [];
    }
    public class WebhookResponse
    {
        [JsonPropertyName("webhooks")] public List<Webhook> Webhooks { get; set; } = [];
    }

    public class Webhook
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("url")] public string Url { get; set; } = string.Empty;

        [JsonPropertyName("event_types")] public List<EventType> EventTypes { get; set; } = [];

        [JsonPropertyName("links")] public List<Link> Links { get; set; } = [];
    }


    #endregion Webhook

}
