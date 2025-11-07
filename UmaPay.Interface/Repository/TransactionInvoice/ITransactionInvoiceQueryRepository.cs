namespace UmaPay.Interface.Repository
{
    using Domain;

    public interface ITransactionInvoiceQueryRepository
    {
        Task<bool> IsValidateStatusAsync(int transactionId, int statusId);
        Task<List<Invoice>> GetLatestPaidInvoicesAsync(List<string> invoiceNumbers);
    }
}