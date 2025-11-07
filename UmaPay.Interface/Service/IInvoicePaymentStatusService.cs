namespace UmaPay.Interface.Service
{
    using Domain;

    public interface IInvoicePaymentStatusService
    {
        Task<InvoicePaymentStatusListResponse> CheckInvoicePaymentStatusAsync(InvoicePaymentStatusRequest request);
    }
}