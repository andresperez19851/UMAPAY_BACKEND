namespace UmaPay.Interface.Repository
{
    using Domain;

    public interface IGatewayCountryCommandRepository
    {
        Task<GatewayCountry> AddAsync(GatewayCountry gatewayApplication);
    }
}
