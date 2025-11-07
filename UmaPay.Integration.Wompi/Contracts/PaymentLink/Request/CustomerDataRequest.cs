using System.Text.Json.Serialization;

namespace UmaPay.Integration.Wompi.Contracts.PaymentLink.Request;

public class CustomerReference
{
    [JsonPropertyName("label")]
    public string Label { get; set; }

    [JsonPropertyName("is_required")]
    public bool IsRequired { get; set; }
}

public class CustomerDataRequest
{
    [JsonPropertyName("customer_references")]
    public List<CustomerReference> CustomerReferences { get; set; }
}