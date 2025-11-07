namespace UmaPay.Interface.Service
{
    using Domain;

    public interface ITransactionCommandHandler
    {
        Task<OperationResult<Transaction>> CreateAsync(Transaction command);
        Task<OperationResult<Transaction>> GenereLinkAsync(Guid tokenCommand, Gateway gatewayCommand, string email);

        /// <summary>
        /// Procesa las facturas de transacción.
        /// </summary>
        /// <param name="transaction">La transacción a procesar.</param>
        Task ProcessTransactionInvoices(Transaction transaction);
    }
}