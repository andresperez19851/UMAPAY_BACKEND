using System.Text.Json.Serialization;

namespace UmaPay.Domain
{
    public class NuveiRequestCardData
    {
        [JsonPropertyName("bin")]
        public string Bin { get; set; }

        [JsonPropertyName("holder_name")]
        public string HolderName { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("number")]
        public string Number { get; set; }

        [JsonPropertyName("origin")]
        public string Origin { get; set; }

        [JsonPropertyName("fiscal_number")]
        public string FiscalNumber { get; set; }
    }
}
