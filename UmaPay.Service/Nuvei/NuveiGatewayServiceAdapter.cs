namespace UmaPay.Service
{
    using UmaPay.Domain;
    using UmaPay.Interface.Service;
    using UmaPay.Interface.Integration.Nuvei;
    using UmaPay.Resource;
    using Microsoft.Extensions.DependencyInjection;

    public class NuveiGatewayServiceAdapter(
        [FromKeyedServices(ConstGateway.GatewayNuvei)] IGeneratePaymentLink nuveiPaymentLinkService,
        [FromKeyedServices(ConstGateway.GatewayNuvei)] IStatusMapper statusMapper) : IGatewayService
    {

        #region Properties

        private readonly IGeneratePaymentLink _nuveiPaymentLinkService = nuveiPaymentLinkService;
        private readonly IStatusMapper _statusMapper = statusMapper;
        public string GatewayCode => ConstGateway.GatewayNuvei;

        #endregion Properties

        #region Public Methods

        public async Task<OperationResult<Transaction>> GeneratePaymentLinkAsync(Transaction transaction)
        {
            try
            {
                var nuveiResponse = await _nuveiPaymentLinkService.GeneratePaymentLinkAsync(transaction);
                if (nuveiResponse.Success)
                {
                    // Actualizar la transacción con la información de Nuvei
                    transaction.PaymentUrl = nuveiResponse.Data.PaymentUrl;
                    transaction.Reference = nuveiResponse.Data.Reference ?? transaction.Reference;
                    transaction.ExpirationDate = nuveiResponse.Data.ExpirationDate;
                    transaction.GatewayResponse = nuveiResponse.Data.GatewayResponse;

                    return OperationResult<Transaction>.SuccessResult(transaction);
                }
                return OperationResult<Transaction>.Failure(nuveiResponse.Message, transaction);
            }
            catch (Exception ex)
            {
                return OperationResult<Transaction>.Failure(string.Format(Message.UnexpectedError, ex.Message));
            }
        }

        public TransactionStatus GetStatus(string gatewayStatus)
        {
            return _statusMapper.MapToInternalStatus(gatewayStatus);
        }



        #endregion Public Methods

    }

}