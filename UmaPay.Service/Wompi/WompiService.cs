using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using UmaPay.Domain;
using UmaPay.Domain.Service.Wompi.Webhook;
using UmaPay.Interface.Integration.Middleware;
using UmaPay.Interface.Repository;
using UmaPay.Interface.Service;
using UmaPay.Resource;

namespace UmaPay.Service.Wompi;

public sealed class WompiService(
    IWompiVerifySignatureService wompiVerifySignatureService,
    ILogger<WompiService> logger,
    ITransactionQueryRepository transactionQueryRepository,
    ITransactionCommandHandler transactionCommandHandler,
    IGatewayServiceFactory gatewayServiceFactory,
    ITransactionCommandRepository transactionCommandRepository,
    ITransactionStatusLogCommandRepository transactionStatusLogCommandRepository,
    ITransactionInvoiceQueryRepository transactionInvoiceQueryRepository,
    ITransactionInvoiceCommandRepository transactionInvoiceCommandRepository,
    IInvoiceService invoiceService) : IWompiService
{
    private readonly IWompiVerifySignatureService _wompiVerifySignatureService = wompiVerifySignatureService;
    private readonly ILogger<WompiService> _logger = logger;
    private readonly ITransactionQueryRepository _transactionQueryRepository = transactionQueryRepository;
    private readonly ITransactionCommandHandler _transactionCommandHandler = transactionCommandHandler;
    private readonly IGatewayServiceFactory _gatewayServiceFactory = gatewayServiceFactory;
    private readonly ITransactionCommandRepository _transactionCommandRepository = transactionCommandRepository;
    private readonly ITransactionStatusLogCommandRepository _transactionStatusLogCommandRepository = transactionStatusLogCommandRepository;
    private readonly ITransactionInvoiceQueryRepository _transactionInvoiceQueryRepository = transactionInvoiceQueryRepository;
    private readonly ITransactionInvoiceCommandRepository _transactionInvoiceCommandRepository = transactionInvoiceCommandRepository;
    private readonly IInvoiceService _invoiceService = invoiceService;

    /// <summary>
    /// Verifica la firma de la solicitud de evento de Wompi.
    /// </summary>
    /// <param name="request">Solicitud de evento de Wompi.</param>
    /// <returns>Resultado de la operación que indica si la firma es válida.</returns>
    public async Task<OperationResult<bool>> VerifySignature(WompiEventRequest request)
    {
        try
        {
            var transaction = await _transactionQueryRepository.GetByGatewayReference(
                request.Data.Transaction.PaymentLinkId!,
                ConstGateway.GatewayWompy);

            if (transaction is null)
            {
                return string.Format(
                    Message.TransactionReferenceRequired,
                    request.Data.Transaction.Reference);
            }

            return _wompiVerifySignatureService.ValidateSignature(transaction.Customer!.Society, request);
        }
        catch (Exception ex)
        {
            var errorMessage = string.Format(Message.UnexpectedError, nameof(VerifySignature));
            _logger.LogError(ex, errorMessage);
            return errorMessage;
        }
    }

    /// <summary>
    /// Procesa la solicitud de evento de Webhook Wompi.
    /// </summary>
    /// <returns>Resultado del proceso con transacción.</returns>
    public async Task<OperationResult<Transaction>> ProcessAsync(
        string rawRequest,
        WompiEventRequest request)
    {
        try
        {
            if (!request.Event.Equals(Message.WompiTransactionUpdatedEvent))
            {
                _logger.LogWarning($"{Message.UnprocessableWompiEvent} {request.Event}");
                return Message.UnprocessableWompiEvent;
            }

            IGatewayService gatewayService = _gatewayServiceFactory.GetGatewayService(ConstGateway.GatewayWompy);

            var transaction = await _transactionQueryRepository.GetByGatewayReference(
                request.Data.Transaction.PaymentLinkId!,
                gatewayService.GatewayCode);

            if (transaction is null)
            {
                return string.Format(
                    Message.TransactionReferenceRequired,
                    request.Data.Transaction.Reference);
            }

            await UpdateTransaction(gatewayService, transaction, rawRequest, request);
            await AddTransactionLog(transaction);
            await _transactionCommandHandler.ProcessTransactionInvoices(transaction);

            return transaction;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, Message.UnexpectedError);
            return Message.UnexpectedError;
        }
    }

    private async Task UpdateTransaction(
        IGatewayService gatewayService,
        Transaction transaction,
        string rawRequest,
        WompiEventRequest request)
    {
        transaction.GatewayPayment = rawRequest;
        //transaction.Reference = request.Data.Transaction.Reference; 
        transaction.Status = gatewayService.GetStatus(request.Data.Transaction.Status);
        await _transactionCommandRepository.UpdateAsync(transaction);
    }

    private async Task AddTransactionLog(Transaction transaction)
    {
        await _transactionStatusLogCommandRepository
            .AddAsync(TransactionStatusLog.Create(transaction));
    }
}
