namespace UmaPay.Interface.Repository
{
    using Domain;
    public interface ITransactionStatusLogCommandRepository
    {
        Task<TransactionStatusLog> AddAsync(TransactionStatusLog command);
    }
}