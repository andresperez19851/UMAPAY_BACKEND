namespace UmaPay.Service
{
    using Domain;
    using Interface.Repository;
    using Interface.Service;
    using Resource;

    public class TransactionQueryHandler : ITransactionQueryHandler
    {
        #region Properties

        private readonly ITransactionQueryRepository _transactionQueryRepository;

        private readonly ITransactionStatusLogCommandRepository _transactionStatusLogCommandRepository;

        #endregion Properties

        #region Constructor

        public TransactionQueryHandler(
             ITransactionQueryRepository transactionCommandRepository, ITransactionStatusLogCommandRepository transactionStatusLogCommandRepository)
        {
            _transactionQueryRepository = transactionCommandRepository ?? throw new ArgumentNullException(nameof(_transactionQueryRepository)); 

            _transactionStatusLogCommandRepository = transactionStatusLogCommandRepository ?? throw new ArgumentNullException(nameof(transactionStatusLogCommandRepository));
        }
        #endregion Constructor

        #region Public Methods

        public async Task<OperationResult<Transaction>> GetByTokenSingleAsync(Guid token, bool? logTransactionStatus = default)
        {
            try
            {
                var transaction = await _transactionQueryRepository.GetByTokenSingleAsync(token);

                if (transaction is null)
                {
                    return Message.TransactionNoFound;
                }

                if (logTransactionStatus.HasValue && logTransactionStatus.Value)
                {
                    await _transactionStatusLogCommandRepository
                        .AddAsync(TransactionStatusLog.Create(transaction));
                }

                return transaction;
            }
            catch (Exception ex)
            {
                return OperationResult<Transaction>.Failure($"{Message.TransactionError} {ex.Message}");
            }
        }

        public async Task<OperationResult<Transaction>> GetByTokenAsync(Guid token, string flag)
        {
            try
            {
                var transaction = await _transactionQueryRepository.GetByTokenAsync(token);

                int status = ConstStatus.GetStatusFromFlag(flag) switch
                {
                    ConstStatus.Completed => ConstStatus.GatewaySucess,
                    ConstStatus.Failed => ConstStatus.GatewayFailure,
                    ConstStatus.Processing => ConstStatus.GatewayPending,
                    ConstStatus.Review => ConstStatus.GatewayReview,
                    _ => 0 
                };

                //Adciona Log
                TransactionStatusLog log = new TransactionStatusLog
                {
                    Transaction = transaction,
                    Status = new TransactionStatus { Id = status, Name = ConstStatus.GetStatusName(status) },
                    Comment = ConstStatus.GetStatusDescription(status),
                    CreatedAt = DateTime.UtcNow
                };
                await _transactionStatusLogCommandRepository.AddAsync(log);

                return transaction != null
                    ? OperationResult<Transaction>.SuccessResult(transaction)
                     : OperationResult<Transaction>.Failure(Message.TransactionNoFound);
            }
            catch (Exception ex)
            {
                return OperationResult<Transaction>.Failure($"{Message.TransactionError} {ex.Message}");
            }
        }

        public async Task<OperationResult<IEnumerable<TransactionReport>>> SearchTransactionsAsync(
            DateTime? startDate, DateTime? endDate, string? status, Guid? token, string? customer)
        {
            try
            {
                var adjustedStartDate = startDate?.Date;

                // Ajustar la fecha final a las 23:59:59 si se proporciona
                var adjustedEndDate = endDate?.Date.AddDays(1).AddTicks(-1);

                var transactions = await _transactionQueryRepository.SearchTransactionsAsync(adjustedStartDate, adjustedEndDate, status, token, customer);
                return OperationResult<IEnumerable<TransactionReport>>.SuccessResult(transactions);
            }
            catch (Exception ex)
            {
                return OperationResult<IEnumerable<TransactionReport>>.Failure($"{Message.TransactionError} {ex.Message}");
            }
        }

        #endregion Public Methods

    }
}