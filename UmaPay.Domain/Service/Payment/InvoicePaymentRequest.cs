

namespace UmaPay.Domain
{
    public class InvoicePaymentRequest
    {
        public required string Number { get; set; }
        public required decimal Amount { get; set; }
        public decimal? Total { get; set; }
    }
}
