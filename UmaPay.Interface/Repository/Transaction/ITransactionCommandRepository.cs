namespace UmaPay.Interface.Repository
{
    using Domain;

    public interface ITransactionCommandRepository
    {
        Task<Transaction> AddAsync(Transaction transaction);
        Task<Transaction> UpdateAsync(Transaction transaction);
        Task DeleteAsync(int id);
    }
}