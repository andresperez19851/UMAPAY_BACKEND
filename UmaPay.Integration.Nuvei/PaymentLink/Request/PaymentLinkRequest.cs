using System.Text.Json.Serialization;

namespace UmaPay.Integration.Nuvei
{
    public class PaymentLinkRequest
    {
        [JsonPropertyName("user")]
        public PaymentLinkRequestCustomer User { get; set; }

        [JsonPropertyName("order")]
        public PaymentLinkRequestOrder Order { get; set; }

        [JsonPropertyName("configuration")]
        public PaymentLinkRequestConfiguration Configuration { get; set; }
    }
}
