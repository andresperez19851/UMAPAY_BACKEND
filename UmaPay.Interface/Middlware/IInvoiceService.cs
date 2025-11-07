namespace UmaPay.Interface.Integration.Middleware
{
    using Domain;

    public interface IInvoiceService
    {
        Task<OperationResult<SapResponse>> ProcessSapPaymentAsync(
            IEnumerable<Invoice> invoices,
            string currency,
            string gatewayCode);
    }
}