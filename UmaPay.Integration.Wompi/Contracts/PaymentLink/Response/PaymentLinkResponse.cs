using System.Text.Json.Serialization;

namespace UmaPay.Integration.Wompi.Contracts.PaymentLink.Response;
public sealed record PaymentLinkResponse(
    [property: JsonPropertyName("data")] PaymentLinkDataResponse Data
);

public sealed record PaymentLinkDataResponse(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("active")] bool Active,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("description")] string Description,
    [property: JsonPropertyName("single_use")] bool SingleUse,
    [property: JsonPropertyName("collect_shipping")] bool CollectShipping,
    [property: JsonPropertyName("collect_customer_legal_id")] bool CollectCustomerLegalId,
    [property: JsonPropertyName("amount_in_cents")] long AmountInCents,
    [property: JsonPropertyName("currency")] string Currency,
    [property: JsonPropertyName("signature")] string Signature,
    [property: JsonPropertyName("reference")] string Reference,
    [property: JsonPropertyName("expiration_time")] DateTime ExpirationTime,
    [property: JsonPropertyName("sku")] string Sku,
    [property: JsonPropertyName("expires_at")] DateTime ExpiresAt,
    [property: JsonPropertyName("redirect_url")] string RedirectUrl,
    [property: JsonPropertyName("image_url")] string ImageUrl,
    [property: JsonPropertyName("customer_data")] CustomerData CustomerData,
    [property: JsonPropertyName("taxes")] List<TaxrResponse> Taxes,
    [property: JsonPropertyName("created_at")] DateTime CreatedAt,
    [property: JsonPropertyName("updated_at")] DateTime UpdatedAt
);

public sealed record CustomerData(
    [property: JsonPropertyName("customer_references")] List<CustomerReferenceResponse> CustomerReferences
);

public sealed record CustomerReferenceResponse(
    [property: JsonPropertyName("label")] string Label,
    [property: JsonPropertyName("is_required")] bool IsRequired
);

public sealed record TaxrResponse(
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("amount_in_cents")] int? AmountInCents,
    [property: JsonPropertyName("percentage")] int? Percentage
);