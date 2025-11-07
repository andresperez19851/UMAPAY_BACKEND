using System.Text.Json.Serialization;

namespace UmaPay.Integration.Nuvei
{
    public class PaymentLinkRequestOrder
    {
        [JsonPropertyName("dev_reference")]
        public string DevReference { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }

        [JsonPropertyName("installments_type")]
        public int InstallmentsType { get; set; }

        [JsonPropertyName("currency")]
        public string Currency { get; set; }
    }

}
