namespace UmaPay.Interface.Repository
{
    using Domain;

    public interface IGatewayApplicationCommandRepository
    {
        Task<GatewayApplication> AddAsync(GatewayApplication gatewayApplication);
        Task<GatewayApplication> UpdateAsync(GatewayApplication gatewayApplication);
        Task DeleteAsync(int gatewayId, int applicationId);
    }
}
