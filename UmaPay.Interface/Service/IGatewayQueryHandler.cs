namespace UmaPay.Interface.Service
{
    using Domain;
    public interface IGatewayQueryHandler
    {
        Task<IEnumerable<Gateway>> GetAllAsync();
        Task<IEnumerable<Gateway>> GetByApplicationAsync(int application);
        Task<Gateway> GetByApplicationAsync(string apikey, string code);
    }
}