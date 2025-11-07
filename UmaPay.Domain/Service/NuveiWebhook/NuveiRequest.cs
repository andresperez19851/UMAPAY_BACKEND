using System.Text.Json.Serialization;

namespace UmaPay.Domain
{
    public class NuveiRequest
    {
        [JsonPropertyName("transaction")]
        public NuveiRequestTransactionData? Transaction { get; set; }

        [JsonPropertyName("user")]
        public NuveiRequestUserData? User { get; set; }

        [JsonPropertyName("card")]
        public NuveiRequestCardData? Card { get; set; }
    }
}
