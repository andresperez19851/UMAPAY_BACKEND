using System.Text.Json.Serialization;

namespace UmaPay.Domain
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum PaymentStatus
    {
        EnProceso,
        PendienteDePago,
        PagoParcial
    }

    public class InvoicePaymentStatusResponse
    {
        public required string Factura { get; set; }
        public required decimal Monto { get; set; }
        public required string Estado { get; set; }
        public required bool Pago { get; set; }
    }

    public class InvoicePaymentStatusListResponse
    {
        public required List<InvoicePaymentStatusResponse> Invoices { get; set; }
    }
}