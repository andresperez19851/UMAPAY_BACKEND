namespace UmaPay.Domain
{
    public class InvoicePaymentStatusRequest
    {
        public required List<InvoicePaymentCheckRequest> Invoices { get; set; }
    }
}