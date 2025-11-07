namespace UmaPay.Interface.Repository
{
    using Domain;

    public interface IGatewayApplicationQueryRepository
    {
        Task<GatewayApplication> GetAsync(int gatewayId, int applicationId);
        Task<IEnumerable<GatewayApplication>> GetByGatewayIdAsync(int gatewayId);
        Task<IEnumerable<GatewayApplication>> GetByApplicationIdAsync(int applicationId);
        Task<IEnumerable<GatewayApplication>> GetAllAsync();
        Task<bool> IsGatewayAssociatedWithApplicationAsync(string apiKey, string gateway);
    }
}
