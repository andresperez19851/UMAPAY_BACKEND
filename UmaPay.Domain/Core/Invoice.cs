namespace UmaPay.Domain
{
    public class Invoice
    {
        public string Number { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        public decimal? Total { get; set; }

        public bool IsPaid { get; set; }

        public decimal? TotalToPay { get; set; }

        public TransactionStatus? Status { get; set; }

    }
}