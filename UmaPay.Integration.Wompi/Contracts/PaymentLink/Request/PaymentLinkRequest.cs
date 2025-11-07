using System.Text.Json.Serialization;

namespace UmaPay.Integration.Wompi.Contracts.PaymentLink.Request;

public sealed class PaymentLinkRequest
{
    /// <summary>
    /// Nombre del link de pago.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; }

    /// <summary>
    /// Descripción del pago.
    /// </summary>
    [JsonPropertyName("description")]
    public string Description { get; set; }

    /// <summary>
    /// Indica si el link de pago es de un solo uso.
    /// `false` current caso de que el link de pago pueda recibir múltiples transacciones APROBADAS o `true` si debe dejar de aceptar transacciones después del primer pago APROBADO.
    /// </summary>
    [JsonPropertyName("single_use")]
    public bool SingleUse { get; set; }

    /// <summary>
    /// Indica si se debe solicitar la dirección de envío del cliente.
    /// </summary>
    [JsonPropertyName("collect_shipping")]
    public bool CollectShipping { get; set; }

    /// <summary>
    /// Indica si se debe solicitar el tipo de documento del cliente
    [JsonPropertyName("collect_customer_legal_id")]
    public bool CollectCustomerLegalId { get; set; }

    /// <summary>
    /// Monto del pago en centavos.
    /// Si el pago es por un monto específico, si no lo incluyes el pagador podrá elegir el valor a pagar
    /// </summary>
    [JsonPropertyName("amount_in_cents")]
    public long AmountInCents { get; set; }

    /// <summary>
    /// Moneda en la que se realiza el pago.
    /// </summary>
    [JsonPropertyName("currency")]
    public string Currency { get; set; }

    [JsonPropertyName("signature")]
    public string? Signature { get; set; }

    [JsonPropertyName("reference")]
    public string? Reference { get; set; }

    /// <summary>
    /// Fecha de expiración del link de pago.
    /// </summary>
    [JsonPropertyName("expiration_time")]
    public DateTime? ExpirationTime { get; set; }

    /// <summary>
    ///  Identificador interno del producto current tu comercio. Máximo 36 caracteres
    /// </summary>
    [JsonPropertyName("sku")]
    public string? Sku { get; set; }

    /// <summary>
    /// Fecha de expiración del link de pago.
    /// </summary>
    [JsonPropertyName("expires_at")]
    public DateTime? ExpiresAt { get; set; }

    /// <summary>
    /// URL a la que se redirigirá al pagador después de realizar el pago.
    /// </summary>
    [JsonPropertyName("redirect_url")]
    public string? RedirectUrl { get; set; }

    /// <summary>
    /// URL de la imagen que se mostrará en el link de pago.
    /// </summary>
    [JsonPropertyName("image_url")]
    public string? ImageUrl { get; set; }

    /// <summary>
    /// Información de cliente (opcional).
    /// Campos personalizados (máximo 2) del cliente antes de realizar la transacción
    /// </summary>
    [JsonPropertyName("customer_data")]
    public CustomerDataRequest? CustomerData { get; set; }

    /// <summary>
    /// Información de impuestos (opcional).
    /// </summary>
    [JsonPropertyName("taxes")]
    public List<TaxRequest>? Taxes { get; set; }

}