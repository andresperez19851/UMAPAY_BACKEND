using System.Text.Json.Serialization;

namespace UmaPay.Integration.Nuvei
{
    public class PaymentLinkResponseAdditionalAmount
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }
    }
}
