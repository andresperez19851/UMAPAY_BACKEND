using System.Text.Json.Serialization;
using System.Text.Json;

namespace UmaPay.Domain
{

    public class SapResponse
    {
        [JsonPropertyName("ET_RETURN")]
        public ET_RETURN ET_RETURN { get; set; } = new ET_RETURN();

        [JsonIgnore]
        public string? DocumentNumber => ET_RETURN.GetDocumentNumber();

        [JsonIgnore]
        public bool IsSuccess => ET_RETURN.HasDocument;

        [JsonIgnore]
        public string? ResponseContent { get; set; } 
    }

    public class ET_RETURN
    {
        [JsonIgnore]
        public List<string> Items { get; } = new List<string>();

        [JsonIgnore]
        public bool HasDocument => Items.Any(x => x.StartsWith("Doc."));

        public string? GetDocumentNumber()
        {
            var docItem = Items.FirstOrDefault(x => x.StartsWith("Doc."));
            if (docItem == null) return null;

            var match = System.Text.RegularExpressions.Regex.Match(docItem, @"Doc\.(\d+)");
            return match.Success ? match.Groups[1].Value : null;
        }

        public string GetFormattedMessage()
        {
            return string.Join(", ", Items);
        }
    }
}