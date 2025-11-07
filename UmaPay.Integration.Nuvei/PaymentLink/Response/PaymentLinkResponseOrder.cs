using System.Text.Json.Serialization;

namespace UmaPay.Integration.Nuvei
{
    public class PaymentLinkResponseOrder
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("dev_reference")]
        public string DevReference { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("currency")]
        public string Currency { get; set; }

        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }

        [JsonPropertyName("taxes")]
        public List<PaymentLinkResponseTax> Taxes { get; set; }

        [JsonPropertyName("additional_amounts")]
        public List<PaymentLinkResponseAdditionalAmount> AdditionalAmounts { get; set; }
    }
}
