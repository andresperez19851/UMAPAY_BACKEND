namespace UmaPay.Domain
{
    public class InvoicePaymentCheckRequest
    {
        public required string Factura { get; set; }
        public required decimal Monto { get; set; }
    }
}