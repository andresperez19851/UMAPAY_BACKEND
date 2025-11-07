namespace UmaPay.Interface.Service
{
    using Domain;

    public interface INuveiCommandHandler
    {
        Task<OperationResult<Transaction>> ProcessAsync(string reference, IGatewayService gatewayService, string rawJson, string id);
        Task<OperationResult<bool>> Verify(string stoken, string transaction, string user);
    }
}