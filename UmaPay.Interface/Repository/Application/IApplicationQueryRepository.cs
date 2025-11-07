namespace UmaPay.Interface.Repository
{
    using Domain;

    public interface IApplicationQueryRepository
    {

        Task<Application> GetByIdAsync(int id);
        Task<Application> GetByNameAsync(string name);
        Task<Application> GetByApiKeyAsync(string apiKey);
        Task<IEnumerable<Application>> GetAllAsync();
        Task<bool> ExistsByNameAsync(string name);
    }
}
