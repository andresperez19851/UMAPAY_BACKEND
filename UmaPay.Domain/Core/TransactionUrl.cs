namespace UmaPay.Domain
{
    public class TransactionUrl
    {
        public string SuccessUrl { get; set; } = string.Empty;
        public string FailureUrl { get; set; } = string.Empty;
        public string PendingUrl { get; set; } = string.Empty;
        public string ReviewUrl { get; set; } = string.Empty;

        public string PaymentUrl { get; set; } = string.Empty;

    }
}