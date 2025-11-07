namespace UmaPay.Interface.Repository
{
    using Domain;

    public interface IGatewayQueryRepository
    {
        Task<Gateway> GetByIdAsync(int id);
        Task<Gateway> GetByNameAsync(string name);
        Task<Gateway> GetByCodeAsync(string code);
        Task<IEnumerable<Gateway>> GetAllAsync();
        Task<IEnumerable<Gateway>> GetByApplicationAsync(int application);
        Task<Gateway> GetByApplicationAsync(string apikey, string code);
        Task<Gateway> GetByApplicationAsync(int application, string code, string currency);
        Task<bool> ExistsGatewayApplicationCountryAsync(int gatewayId, int applicationId, int countryId);
    }
}
