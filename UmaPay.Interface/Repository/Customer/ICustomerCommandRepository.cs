namespace UmaPay.Interface.Repository
{
    using Domain;
    public interface ICustomerCommandRepository
    {
        Task<Customer> AddAsync(Customer customer);
        Task<Customer> UpdateAsync(Customer customer);
        Task DeleteAsync(int id);
    }
}
