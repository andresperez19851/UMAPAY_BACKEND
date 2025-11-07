namespace UmaPay.Domain
{
    public class InvoiceResponse
    {
        public string Number { get; set; }
        public decimal Amount { get; set; }
        public TransactionStatusResponse? Status { get; set; }
    }
}
