namespace UmaPay.Integration.Wompi.Enums;

internal enum WompiTransactionStatusEnum
{
    /// <summary>
    /// Transacción aprobada.
    /// </summary>
    APPROVED = 1,

    /// <summary>
    /// Transacción pendiente.
    /// </summary>
    PENDING = 2,

    /// <summary>
    /// Transacción rechazada.
    /// </summary>
    DECLINED = 3,

    /// <summary>
    /// Transacción anulada (sólo aplica para transacciones con tarjeta).
    /// </summary>
    VOIDED = 4,

    /// <summary>
    /// Error interno del método de pago respectivo.
    /// </summary>
    ERROR = 5
}