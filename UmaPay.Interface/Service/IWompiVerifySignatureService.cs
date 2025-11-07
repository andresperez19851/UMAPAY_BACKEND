using UmaPay.Domain.Service.Wompi.Webhook;

namespace UmaPay.Interface.Service;

/// <summary>
/// Interfaz para el servicio de verificación de firma de Wompi.
/// </summary>
public interface IWompiVerifySignatureService
{
    /// <summary>
    /// Valida la firma de un evento de webhook de Wompi.
    /// </summary>
    /// <param name="webhookEvent">El evento de webhook de Wompi.</param>
    /// <param name="society">La sociedad para la cual se valida la firma.</param>
    /// <returns>True si la firma es válida, de lo contrario False.</returns>
    bool ValidateSignature(string society, WompiEventRequest webhookEvent);
}
