using System.Text.Json.Serialization;

namespace UmaPay.Integration.Wompi.Contracts.PaymentLink.Response;
public sealed record ErrorResponse(
    [property: JsonPropertyName("error")] ErrorDetail Error
);

public sealed record ErrorDetail(
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("messages")] Dictionary<string, List<string>> Messages
);