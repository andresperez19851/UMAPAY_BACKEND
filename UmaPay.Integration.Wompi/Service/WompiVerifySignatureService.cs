using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;
using UmaPay.Domain.Service.Wompi.Webhook;
using UmaPay.Integration.Wompi.Resource;
using UmaPay.Integration.Wompi.Settings;
using UmaPay.Interface.Service;

namespace UmaPay.Integration.Wompi.Service;

/// <summary>
/// Servicio para verificar la firma de eventos Wompi.
/// </summary>
public sealed class WompiVerifySignatureService(
    IOptions<WompiSettings> wompiSettings,
    ILogger<WompiVerifySignatureService> logger) : IWompiVerifySignatureService
{
    private readonly WompiSettings _wompiSettings = wompiSettings.Value;
    private readonly ILogger<WompiVerifySignatureService> _logger = logger;

    /// <summary>
    /// Valida la firma de un evento de Wompi.
    /// </summary>
    /// <param name="society">La sociedad para la cual se valida la firma.</param>
    /// <param name="webhookEvent">El evento de Wompi.</param>
    /// <returns><c>true</c> si la firma es válida; de lo contrario, <c>false</c>.</returns>
    public bool ValidateSignature(string society, WompiEventRequest webhookEvent)
    {
        if (!_wompiSettings.Societies!.TryGetValue(society, out var societySettings))
        {
            _logger.LogWarning(PaymentLinkMessages.SocietyNotFound(society));
            return false;
        }

        var computedChecksum = ComputeTransactionChecksum(
            webhookEvent.Data.Transaction,
            webhookEvent.Signature.Properties,
            webhookEvent.Timestamp,
            societySettings.EventSecret!);

        return string.Equals(
            computedChecksum,
            webhookEvent.Signature.Checksum,
            StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Calcula el checksum de una transacción.
    /// </summary>
    /// <param name="transaction">La transacción.</param>
    /// <param name="properties">Las propiedades a concatenar.</param>
    /// <param name="timeStamp">El timestamp de la transacción.</param>
    /// <param name="eventSecret">El secreto del evento.</param>
    /// <returns>El checksum calculado.</returns>
    private string ComputeTransactionChecksum(WompiTransaction transaction, List<string> properties, long timeStamp, string eventSecret)
    {
        var concatenatedProperties = BuildConcatenatedProperties(transaction, properties);
        var concatenatedString = $"{concatenatedProperties}{timeStamp}{eventSecret}";
        using var sha256 = SHA256.Create();
        var computedHash = sha256.ComputeHash(Encoding.UTF8.GetBytes(concatenatedString));
        return BitConverter.ToString(computedHash).Replace("-", "");
    }

    /// <summary>
    /// Construye una cadena concatenada de propiedades.
    /// </summary>
    /// <param name="transaction">La transacción.</param>
    /// <param name="properties">Las propiedades a concatenar.</param>
    /// <returns>La cadena concatenada de propiedades.</returns>
    private string BuildConcatenatedProperties(WompiTransaction transaction, List<string> properties)
    {
        return string.Join(
            string.Empty,
            properties.Select(property => GetPropertyValue(transaction, property)));
    }

    /// <summary>
    /// Obtiene el valor de una propiedad de una transacción.
    /// </summary>
    /// <param name="transaction">La transacción.</param>
    /// <param name="propertyName">El nombre de la propiedad.</param>
    /// <returns>El valor de la propiedad.</returns>
    private string GetPropertyValue(WompiTransaction transaction, string propertyName)
    {
        var propertyPath = propertyName.Split('.');
        var propertyValue = (object)transaction;

        foreach (var prop in propertyPath)
        {
            if (propertyValue == null) return string.Empty;

            var propertyInfo = propertyValue.GetType().GetProperties()
                .FirstOrDefault(p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name == prop);

            if (propertyInfo == null) continue;

            propertyValue = propertyInfo.GetValue(propertyValue);
        }

        return propertyValue?.ToString() ?? string.Empty;
    }
}
