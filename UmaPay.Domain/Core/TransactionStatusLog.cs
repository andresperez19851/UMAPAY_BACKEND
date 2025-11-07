using UmaPay.Resource;

namespace UmaPay.Domain
{
    public class TransactionStatusLog
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? Comment { get; set; }
        public Transaction? Transaction { get; set; }
        public TransactionStatus? Status { get; set; }

        public static TransactionStatusLog Create(Transaction transaction)
        {
            return new TransactionStatusLog
            {
                Transaction = transaction,
                Status = transaction.Status,
                Comment = ConstStatus.GetStatusDescription(transaction.Status.Id),
                CreatedAt = DateTime.UtcNow
            };
        }   

    }
}
