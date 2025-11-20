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
        public string? GatewayRequest { get; set; }
        public string? GatewayResponse { get; set; }
        public string? GatewayPayment { get; set; }
        public DateTime? SapDate { get; set; }
        public string? SapDocument { get; set; }
        public string? SapRequest { get; set; }
        public string? SapResponse { get; set; }
        public List<InvoiceResponse>? Invoices { get; set; }
    }

}
