using System.Text.Json.Serialization;

namespace UmaPay.Domain.Service.Wompi.Webhook;

public sealed class WompiTransactionData
{
    [JsonPropertyName("transaction")]
    public WompiTransaction Transaction { get; set; }
}

public sealed class WompiTransaction
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("amount_in_cents")]
    public long AmountInCents { get; set; }

    [JsonPropertyName("reference")]
    public string Reference { get; set; }

    [JsonPropertyName("customer_email")]
    public string CustomerEmail { get; set; }

    [JsonPropertyName("currency")]
    public string Currency { get; set; }

    [JsonPropertyName("payment_method_type")]
    public string PaymentMethodType { get; set; }

    [JsonPropertyName("redirect_url")]
    public string RedirectUrl { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; }

    [JsonPropertyName("shipping_address")]
    public string? ShippingAddress { get; set; }

    [JsonPropertyName("payment_link_id")]
    public string? PaymentLinkId { get; set; }

    [JsonPropertyName("payment_source_id")]
    public string? PaymentSourceId { get; set; }
}
