using System.Text.Json.Serialization;

namespace UmaPay.Integration.Nuvei
{
    public class PaymentLinkRequestCustomer
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("last_name")]
        public string LastName { get; set; }
    }
}
