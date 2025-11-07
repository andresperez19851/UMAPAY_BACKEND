namespace UmaPay.Service
{
    using UmaPay.Domain;
    using UmaPay.Interface.Repository;
    using UmaPay.Interface.Integration.Middleware;
    using UmaPay.Resource;

    public class InvoceCommandHandler
    {
        #region Properties

        private readonly ITransactionQueryRepository _transactionQueryRepository;
        private readonly ITransactionCommandRepository _transactionCommandRepository;
        private readonly IInvoiceService _invoiceService;

        #endregion Properties

        #region Constructor

        public InvoceCommandHandler(
            ITransactionQueryRepository transactionQueryRepository,
            ITransactionCommandRepository transactionCommandRepository,
            IInvoiceService invoiceService)
        {
            _transactionQueryRepository = transactionQueryRepository;
            _transactionCommandRepository = transactionCommandRepository;
            _invoiceService = invoiceService;
        }

        #endregion Constructor

        #region Public Methods

        public async Task<OperationResult<bool>> PendingTransactionsAsync()
        {
            try
            {
                var failedTransactions = await _transactionQueryRepository.GetTransactionsByStatusAsync(ConstStatus.FailedInSap);
                foreach (var transaction in failedTransactions)
                {
                    var result = await SendTransactionAsync(transaction);
                }
                return OperationResult<bool>.SuccessResult(true);
            }
            catch (Exception ex)
            {
                return OperationResult<bool>.Failure($"Error al reintentar sincronización con ERP: {ex.Message}");
            }
        }

        #endregion Public Methods

        #region Private Methods

        private async Task<OperationResult<bool>> SendTransactionAsync(Transaction transaction)
        {
            try
            {
                /*
                var invoiceUpdateResult = null; await _invoiceService.UpdateInvoicesAsync(transaction);

                if (invoiceUpdateResult.Success)
                {
                    transaction.Status = new TransactionStatus { Id = ConstStatus.CompletedInSap, Name = ConstStatus.GetStatusName(ConstStatus.CompletedInSap) }; ;
                    await _transactionCommandRepository.UpdateAsync(transaction);
                    return OperationResult<bool>.SuccessResult(true);
                }
                else
                {
                    // Opcionalmente, podrías incrementar un contador de intentos aquí
                    await _transactionCommandRepository.UpdateAsync(transaction);
                    return OperationResult<bool>.Failure($"Falló la sincronización con ERP para la transacción {transaction.Id}");
                }
                */
                return OperationResult<bool>.Failure($"Falló la sincronización con ERP para la transacción {transaction.Id}");
            }
            catch (Exception ex)
            {
                return OperationResult<bool>.Failure($"Error al reintentar sincronización con ERP para la transacción {transaction.Id}: {ex.Message}");
            }
        }

        #endregion Private Methods

    }


}