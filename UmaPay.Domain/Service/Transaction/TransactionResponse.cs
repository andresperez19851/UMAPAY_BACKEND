namespace UmaPay.Domain
{
    public class TransactionResponse
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public Guid Token { get; set; }
        public string Reference { get; set; } = string.Empty;
        public TransactionStatusResponse? Status { get; set; }
        public CustomerResponse? Customer { get; set; }
        public GatewayResponse? Gateway { get; set; }
        public Country? Country { get; set; }
        public DateTime TransactionDate { get; set; }
        public string PaymentUrl { get; set; } = string.Empty;
        public List<InvoiceResponse>? Invoices { get; set; }
    }

}
