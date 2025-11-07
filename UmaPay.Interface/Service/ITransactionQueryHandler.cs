namespace UmaPay.Interface.Service
{
    using Domain;

    public interface ITransactionQueryHandler
    {
        Task<OperationResult<Transaction>> GetByTokenAsync(Guid token, string flag);
        Task<OperationResult<Transaction>> GetByTokenSingleAsync(Guid token, bool? logTransactionStatus = default);
        Task<OperationResult<IEnumerable<TransactionReport>>> SearchTransactionsAsync(DateTime? startDate, DateTime? endDate, string? status, Guid? token, string? customer);
    }
}