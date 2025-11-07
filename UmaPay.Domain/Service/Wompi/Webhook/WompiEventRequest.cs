using System.Text.Json.Serialization;

namespace UmaPay.Domain.Service.Wompi.Webhook;

/// <summary>
/// Cuerpo para solicitud de evento de Wompi.
/// </summary>
public sealed class WompiEventRequest
{
    /// <summary>
    /// Nombre del evento. 
    /// </summary>
    [JsonPropertyName("event")] 
    public string Event { get; set; }

    /// <summary>
    /// Datos ransacción de Wompi.
    /// </summary>
    [JsonPropertyName("data")]
    public WompiTransactionData Data { get; set; }

    /// <summary>
    /// Entorno de desarrollo. "test" para Sandbox, "prod" para Producción.
    /// </summary>
    [JsonPropertyName("environment")]
    public string Environment { get; set; }

    /// <summary>
    /// Fima de Wompi.
    /// </summary>
    [JsonPropertyName("signature")]
    public WompiSignature Signature { get; set; }

    /// <summary>
    /// Timestamp UNIX del evento usado para la firma del mismo.
    /// </summary>
    [JsonPropertyName("timestamp")]
    public long Timestamp { get; set; }

    /// <summary>
    /// Fecha current la que se notificó el evento por primera vez.
    /// </summary>
    [JsonPropertyName("sent_at")]
    public DateTime SentAt { get; set; }
}

