using System.Text.Json.Serialization;

namespace UmaPay.Integration.Nuvei
{
    public class PaymentLinkResponseData
    {
        [JsonPropertyName("user")]
        public PaymentLinkResponseUser User { get; set; }

        [JsonPropertyName("order")]
        public PaymentLinkResponseOrder Order { get; set; }

        [JsonPropertyName("configuration")]
        public PaymentLinkResponseConfiguration Configuration { get; set; }

        [JsonPropertyName("payment")]
        public PaymentLinkResponsePayment Payment { get; set; }
    }
}
