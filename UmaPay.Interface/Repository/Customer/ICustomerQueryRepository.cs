namespace UmaPay.Interface.Repository
{
    using Domain;
    public interface ICustomerQueryRepository
    {
        Task<Customer> GetByIdAsync(int id);
        Task<Customer> GetByEmailAsync(string email);
        Task<Customer> GetByCodeSapAsync(string codeSap);
        Task<IEnumerable<Customer>> GetAllAsync();
    }
}
