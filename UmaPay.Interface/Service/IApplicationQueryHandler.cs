namespace UmaPay.Interface.Service
{
    using UmaPay.Domain;

    public interface IApplicationQueryHandler
    {
        Task<IEnumerable<Application>> GetAllAsync();
        Task<Application> GetByApiKeyAsync(string apiKey);
        Task<Application> GetByIdAsync(int id);
    }
}