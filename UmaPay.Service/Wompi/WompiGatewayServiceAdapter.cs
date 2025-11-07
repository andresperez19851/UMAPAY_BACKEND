using Microsoft.Extensions.DependencyInjection;
using UmaPay.Domain;
using UmaPay.Interface.Integration.Nuvei;
using UmaPay.Interface.Service;
using UmaPay.Resource;

namespace UmaPay.Service.Wompi;

public sealed class WompiGatewayServiceAdapter(
    [FromKeyedServices(ConstGateway.GatewayWompy)]IGeneratePaymentLink paymentLinkService,
    [FromKeyedServices(ConstGateway.GatewayWompy)]IStatusMapper statusMapper)
    : IGatewayService
{
    public string GatewayCode => ConstGateway.GatewayWompy;

    private readonly IGeneratePaymentLink _paymentLinkService = paymentLinkService;
    private readonly IStatusMapper _statusMapper = statusMapper;

    public async Task<OperationResult<Transaction>> GeneratePaymentLinkAsync(Transaction transaction)
    {

        var generatePaymentLinkResult = await _paymentLinkService.GeneratePaymentLinkAsync(transaction);
        
        if (!generatePaymentLinkResult.Success)
        {
            return OperationResult<Transaction>.Failure(generatePaymentLinkResult.Message, transaction);
        }

        transaction.Url!.PaymentUrl = generatePaymentLinkResult.Data.PaymentUrl;
        return transaction;
    }

    public TransactionStatus GetStatus(string gatewayStatus)
    {
        return _statusMapper.MapToInternalStatus(gatewayStatus);
    }
}