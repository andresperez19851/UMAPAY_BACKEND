namespace UmaPay.Domain
{
    public class TransactionSearchModel
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Status { get; set; }
        public Guid? Token { get; set; }
        public string? Customer { get; set; }
    }
}

