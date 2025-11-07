using System.Text.Json;

namespace UmaPay.Service
{
    using UmaPay.Domain;
    using UmaPay.Interface.Repository;
    using UmaPay.Interface.Service;
    using UmaPay.Interface.Shared;
    using UmaPay.Resource;
    using UmaPay.Interface.Integration.Nuvei;
    using UmaPay.Interface.Integration.Middleware;
    using System.Net.Mail;
    using UmaPay.Shared;

    public class NuveiCommandHandler : INuveiCommandHandler
    {
        #region Properties

        private readonly ITransactionCommandRepository _transactionCommandRepository;
        private readonly ITransactionQueryRepository _transactionQueryRepository;
        private readonly ITransactionCommandHandler _transactionCommandHandler;
        private readonly ITransactionInvoiceCommandRepository _transactionInvoiceCommandRepository;
        private readonly ITransactionInvoiceQueryRepository _transactionInvoiceQueryRepository;
        private readonly IAuthTokenService _token;
        private readonly ITransactionStatusLogCommandRepository _transactionStatusLogCommandRepository;

        #endregion Properties

        #region Constructor

        public NuveiCommandHandler(
            ITransactionCommandRepository transactionCommandRepository,
            ITransactionQueryRepository transactionQueryRepository,
            ITransactionInvoiceCommandRepository transactionInvoiceCommandRepository,
            ITransactionInvoiceQueryRepository transactionInvoiceQueryRepository,
            IAuthTokenService token,
            ITransactionStatusLogCommandRepository transactionStatusLogCommandRepository
,
            ITransactionCommandHandler transactionCommandHandler)
        {
            _transactionCommandRepository = transactionCommandRepository;
            _transactionQueryRepository = transactionQueryRepository;
            _transactionInvoiceCommandRepository = transactionInvoiceCommandRepository;
            _transactionInvoiceQueryRepository = transactionInvoiceQueryRepository;
            _token = token;
            _transactionStatusLogCommandRepository = transactionStatusLogCommandRepository ?? throw new ArgumentNullException(nameof(transactionStatusLogCommandRepository));
            _transactionCommandHandler = transactionCommandHandler;
        }

        #endregion Constructor

        #region Public Methods

        public async Task<OperationResult<Transaction>> ProcessAsync(string reference, IGatewayService gatewayService, string rawJson, string id)
        {
            try
            {
                var transaction = await _transactionQueryRepository.GetByReferenceAndGatewayAsync(reference, gatewayService.GatewayCode);

                if (transaction == null)
                {
                    return OperationResult<Transaction>.Failure(string.Format(Message.TransactionReferenceRequired, reference));
                }

                var nuveiRequest = JsonSerializer.Deserialize<NuveiRequest>(rawJson);

                transaction.Reference = id;
                transaction.Status = gatewayService.GetStatus(nuveiRequest!.Transaction.StatusDetail);
                transaction.GatewayPayment = rawJson;
                transaction = await _transactionCommandRepository.UpdateAsync(transaction);

                TransactionStatusLog logHub = new TransactionStatusLog
                {
                    Transaction = transaction,
                    Status = transaction.Status,
                    CreatedAt = DateTime.UtcNow
                };

                await _transactionStatusLogCommandRepository.AddAsync(logHub);
                await _transactionCommandHandler.ProcessTransactionInvoices(transaction);
            
                return transaction;
            }
            catch (Exception ex)
            {
                return OperationResult<Transaction>.Failure(string.Format(Message.UnexpectedError, ex.Message));
            }
        }

        public async Task<OperationResult<bool>> Verify(string stoken, string transaction, string user)
        {
            try
            {
                var isValid = _token.VerifyStoken(stoken, transaction, user);
                return OperationResult<bool>.SuccessResult(isValid);
            }
            catch (Exception ex)
            {
                return OperationResult<bool>.Failure(string.Format(Message.UnexpectedError, ex.Message));
            }
        }


        #endregion Public Methods
    }
}