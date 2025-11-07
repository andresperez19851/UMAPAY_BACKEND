namespace UmaPay.Interface.Repository
{
    using Domain;

    public interface IApplicationCommandRepository
    {
        Task<Application> AddAsync(Application command);
        Task<Application> UpdateAsync(Application command);
        Task DeleteAsync(int id);
    }
}
