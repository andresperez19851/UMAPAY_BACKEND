namespace UmaPay.Interface.Service
{
    using Domain;
    public interface IGatewayCommandHandler
    {
        Task<OperationResult<bool>> CreateAsync(string applicationName, string gatewayName, string countryName);
    }
}