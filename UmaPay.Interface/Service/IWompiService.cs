using UmaPay.Domain;
using UmaPay.Domain.Service.Wompi.Webhook;

namespace UmaPay.Interface.Service;

/// <summary>
/// Interfaz para el servicio de Wompi.
/// </summary>
public interface IWompiService
{
    /// <summary>
    /// Verifica la firma de un evento de Wompi.
    /// </summary>
    /// <param name="webhookEvent">El evento de Wompi.</param>
    /// <returns>El resultado de la operación de verificación.</returns>
    Task<OperationResult<bool>> VerifySignature(WompiEventRequest webhookEvent);

    /// <summary>
    /// Procesa de forma asíncrona evento de Wompi.
    /// </summary>
    /// <param name="rawRequest">La solicitud en formato crudo.</param>
    /// <param name="request">La solicitud de Wompi.</param>
    /// <returns>El resultado de la operación de procesamiento con transacción.</returns>
    Task<OperationResult<Transaction>> ProcessAsync(string rawRequest, WompiEventRequest request);
}