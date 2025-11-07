namespace UmaPay.Interface.Repository
{
    using Domain;

    public interface ITransactionInvoiceCommandRepository
    {
        Task<Invoice> AddAsync(int id, Invoice command);
        Task<Invoice> UpdateAsync(int transaction, Invoice command);
    }
}