namespace UmaPay.Interface.Repository
{
    using Domain;

    public interface ICountryQueryRepository
    {
        Task<Country> GetByIdAsync(int id);
        Task<Country> GetByNameAsync(string name);
        Task<IEnumerable<Country>> GetAllAsync();
    }
}
