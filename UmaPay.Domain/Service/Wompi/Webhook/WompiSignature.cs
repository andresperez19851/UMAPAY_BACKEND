using System.Text.Json.Serialization;

namespace UmaPay.Domain.Service.Wompi.Webhook;

/// <summary>
/// Representa la firma de Wompi.
/// </summary>
public sealed class WompiSignature
{
    /// <summary>
    /// Lista de propiedades con las que se construye la firma.
    /// </summary>
    [JsonPropertyName("properties")]
    public List<string> Properties { get; set; }

    /// <summary>
    /// Hash calculado con una firma asimétrica SHA256.
    /// </summary>
    [JsonPropertyName("checksum")]
    public string Checksum { get; set; }
}