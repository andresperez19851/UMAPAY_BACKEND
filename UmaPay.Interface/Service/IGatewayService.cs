namespace UmaPay.Interface.Service;

using Domain;

public interface IGatewayService
{
    string GatewayCode { get; }
    Task<OperationResult<Transaction>> GeneratePaymentLinkAsync(Transaction transaction);
    TransactionStatus GetStatus(string gatewayStatus);
}
