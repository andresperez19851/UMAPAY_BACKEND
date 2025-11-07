using System.Text.Json.Serialization;

namespace UmaPay.Integration.Wompi.Contracts.PaymentLink.Request;

/// <summary>
/// Impuesto de transacción.
/// </summary>
public class TaxRequest
{
    /// <summary>
    /// Tipo de impuesto. (Valores permitidos "VAT" para IVA y "CONSUMPTION" para impuesto al consumo)
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; }

    /// <summary>
    /// Monto del impuesto en centavos.
    /// </summary>
    [JsonPropertyName("amount_in_cents")]
    public int? AmountInCents { get; set; }

    /// <summary>
    /// Porcentaje del impuesto.
    /// </summary>
    [JsonPropertyName("percentage")]
    public int? Percentage { get; set; }
}