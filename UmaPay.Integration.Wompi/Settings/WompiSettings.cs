namespace UmaPay.Integration.Wompi.Settings;

/// <summary>
/// Configuración de Wompi.
/// </summary>
public sealed class WompiSettings
{
    /// <summary>
    /// La dirección base de Wompi.
    /// </summary>
    public string? BaseAddress { get; init; } = null!;

    /// <summary>
    /// El tiempo de espera máximo en segundos para las solicitudes a Wompi.
    /// </summary>
    public int? TimeoutInSeconds { get; init; } = null!;

    /// <summary>
    /// La cantidad de días de expiración para las transacciones en Wompi.
    /// </summary>
    public double ExpirationInDays { get; init; } 

    /// <summary>
    /// El nombre de la transacción en wompi.
    /// </summary>
    public string? TransactionName { get; init; } = null!;

    /// <summary>
    /// Url de redirección en caso de éxito.
    /// </summary>
    public string? CheckOutUrl { get; init; } = null!;

    /// <summary>
    /// Url de redirección al terminar el pago.
    /// </summary>
    public string? RedirectUrl { get; set; } = null!;
    
    /// <summary>
    /// Configuración de sociedades Wompi.
    /// </summary>
    public Dictionary<string, SocietySettings>? Societies { get; set; }
}

/// <summary>
/// Configuración de una sociedad Wompi.
/// </summary>
public sealed class SocietySettings
{
    /// <summary>
    /// Código sociedad.
    /// </summary>
    public string Society { get; init; } = null!;

    /// <summary>
    /// Llave pública de Wompi.
    /// </summary>
    public string PublicKey { get; init; } = null!;

    /// <summary>
    /// Llave privada de Wompi.
    /// </summary>
    public string PrivateKey { get; init; } = null!;

    /// <summary>
    /// Secreto para manejo de eventos.
    /// </summary>
    public string? EventSecret { get; set; } = null!;

}