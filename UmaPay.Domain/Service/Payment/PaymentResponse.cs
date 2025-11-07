namespace UmaPay.Domain
{
    public class PaymentResponse
    {
        public int TransactionId { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
        public string PaymentLink { get; set; }
    }
}
