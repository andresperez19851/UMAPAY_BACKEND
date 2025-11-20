namespace UmaPay.Interface.Repository
{
    using Domain;

    public interface ITransactionQueryRepository
    {
        Task<Transaction> GetByIdAsync(int id);
        Task<IEnumerable<Transaction>> GetByStatusAsync(int statusId);
        Task<IEnumerable<Transaction>> GetAllAsync();
        Task<Transaction> GetByReferenceAndGatewayAsync(string reference, string gatewayCode);
        Task<IEnumerable<Transaction>> GetTransactionsByStatusAsync(int status);
        Task<IEnumerable<Transaction>> GetTransactionsByStatusAsync(int status, string customer);
        Task<Transaction> GetTransactionsByStatusAsync(int status, Guid token);
        Task<Transaction> GetByTokenAsync(Guid token);
        Task<Transaction> GetByTokenSingleAsync(Guid token);
        Task<Transaction> GetByTokenCompleteAsync(Guid token);
        Task<bool> IsTokenUniqueAsync(Guid token);
        Task<IEnumerable<TransactionReport>> SearchTransactionsAsync(DateTime? startDate, DateTime? endDate, string? status, Guid? token, string customer);
        Task<Transaction?> GetByGatewayReference(string gatewayReferenceId, string gatewayCode);
    }

}