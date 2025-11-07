using System.Text.Json.Serialization;

namespace UmaPay.Integration.Nuvei
{
    public class PaymentLinkResponseConfiguration
    {
        [JsonPropertyName("expiration_date")]
        public string ExpirationDate { get; set; }

        [JsonPropertyName("partial_payment")]
        public bool PartialPayment { get; set; }

        [JsonPropertyName("allowed_payment_methods")]
        public List<string> AllowedPaymentMethods { get; set; }

        [JsonPropertyName("allow_retry")]
        public bool AllowRetry { get; set; }
    }
}
