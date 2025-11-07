
namespace UmaPay.Integration.Wompi.Resource;

public static class PaymentLinkMessages
{
    public static string ErrorCreatingPaymentLink(int transactionId, int statusCode)
    {
        return $"Error al crear enlace de pago de Wompi para la transacción {transactionId}. Código de estado: {statusCode}";
    }

    public static string ErrorAtCreatingPaymentLink => "Error al generar enlace de pago en la pasarela Wompi.";

    public static string GeneratingPaymentLink(int transactionId)
    {
        return $"Generando enlace de pago a través de Wompi para la transacción {transactionId}";
    }

    public static string SuccessfullyGeneratedPaymentLink(int transactionId)
    {
        return $"Enlace de pago generado exitosamente a través de Wompi para la transacción {transactionId}.";
    }

    public static string? SocietyNotFound(string society)
    {
        return $"No se encontró la sociedad {society} en la configuración de Wompi.";
    }
}
