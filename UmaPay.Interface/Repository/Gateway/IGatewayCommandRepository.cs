namespace UmaPay.Interface.Repository
{
    using Domain;

    public interface IGatewayCommandRepository
    {
        Task<Gateway> AddAsync(Gateway gateway);
        Task<Gateway> UpdateAsync(Gateway gateway);
        Task DeleteAsync(int id);
    }
}
