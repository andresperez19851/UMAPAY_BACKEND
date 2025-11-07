namespace UmaPay.Domain
{
    public class Transaction
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public DateTime? SapDate { get; set; }
        public string? Description { get; set; }
        public Guid Token { get; set; }
        public TransactionStatus? Status { get; set; }
        public TransactionStatus? StatusPayment { get; set; }
        public Gateway? Gateway { get; set; }
        public Customer? Customer { get; set; }
        public Country? Country { get; set; }

        public string GatewayResponse { get; set; } = string.Empty;
        public string GatewayRequest { get; set; } = string.Empty;
        public string GatewayPayment { get; set; } = string.Empty;
        public string? SapResponse { get; set; }

        public ICollection<Invoice>? Invoice { get; set; }

        public TransactionUrl? Url { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string Reference { get; set; } = string.Empty;
        public string PaymentUrl { get; set; } = string.Empty;

        public string SapDocument { get; set; } = string.Empty;

    }
}