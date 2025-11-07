namespace UmaPay.Interface.Repository
{
    using Domain;

    public interface ICountryCommandRepository
    {
        Task<Country> AddAsync(Country country);
        Task<Country> UpdateAsync(Country country);
        Task DeleteAsync(int id);
    }
}
