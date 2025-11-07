using System.Text.Json.Serialization;

namespace UmaPay.Integration.Nuvei
{
    public class PaymentLinkRequestConfiguration
    {
        [JsonPropertyName("partial_payment")]
        public bool PartialPayment { get; set; }

        [JsonPropertyName("expiration_days")]
        public int ExpirationDays { get; set; }

        [JsonPropertyName("allowed_payment_methods")]
        public List<string> AllowedPaymentMethods { get; set; }

        [JsonPropertyName("success_url")]
        public string SuccessUrl { get; set; }

        [JsonPropertyName("failure_url")]
        public string FailureUrl { get; set; }

        [JsonPropertyName("pending_url")]
        public string PendingUrl { get; set; }

        [JsonPropertyName("review_url")]
        public string ReviewUrl { get; set; }
    }
}
