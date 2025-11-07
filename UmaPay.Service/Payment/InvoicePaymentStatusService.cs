using Microsoft.Extensions.Logging;

namespace UmaPay.Service.Payment
{
    using Domain;
    using Interface.Repository;
    using Interface.Service;

    public class InvoicePaymentStatusService : IInvoicePaymentStatusService
    {
        private readonly ITransactionInvoiceQueryRepository _transactionInvoiceRepository;
        private readonly ILogger<InvoicePaymentStatusService> _logger;

        public InvoicePaymentStatusService(
            ITransactionInvoiceQueryRepository transactionInvoiceRepository,
            ILogger<InvoicePaymentStatusService> logger)
        {
            _transactionInvoiceRepository = transactionInvoiceRepository ?? throw new ArgumentNullException(nameof(transactionInvoiceRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<InvoicePaymentStatusListResponse> CheckInvoicePaymentStatusAsync(InvoicePaymentStatusRequest request)
        {
            try
            {
                _logger.LogInformation("Checking payment status for {InvoiceCount} invoices", request.Invoices.Count);

                var invoiceNumbers = request.Invoices.Select(i => i.Factura).ToList();
                var paidInvoices = await _transactionInvoiceRepository.GetLatestPaidInvoicesAsync(invoiceNumbers);

                var responses = new List<InvoicePaymentStatusResponse>();

                foreach (var invoice in request.Invoices)
                {
                    var paidInvoice = paidInvoices.FirstOrDefault(pi => pi.Number == invoice.Factura);

                    if (paidInvoice == null)
                    {
                        // No hay pagos para esta factura
                        responses.Add(new InvoicePaymentStatusResponse
                        {
                            Factura = invoice.Factura,
                            Monto = invoice.Monto,
                            Estado = "",
                            Pago = true // Verdadero cuando no tiene ningun pago
                        });
                    }
                    else
                    {
                        // Hay pagos, verificar el estado usando los nuevos campos
                        var totalPaid = paidInvoice.Total ?? paidInvoice.Amount;

                        if (string.IsNullOrEmpty(paidInvoice.Number))
                        {
                            // No hay pagos para esta factura
                            responses.Add(new InvoicePaymentStatusResponse
                            {
                                Factura = invoice.Factura,
                                Monto = invoice.Monto,
                                Estado = "",
                                Pago = true // Verdadero cuando no tiene ningun pago
                            });
                        }
                        else
                        {
                            if (paidInvoice.IsPaid)
                            {
                                // Pago completo
                                responses.Add(new InvoicePaymentStatusResponse
                                {
                                    Factura = invoice.Factura,
                                    Monto = invoice.Monto,
                                    Estado = "Pago en proceso",
                                    Pago = false // Falso cuando fue pagada completamente
                                });
                            }
                            else
                            {
                                // Pago parcial
                                responses.Add(new InvoicePaymentStatusResponse
                                {
                                    Factura = invoice.Factura,
                                    Monto = invoice.Monto,
                                    Estado = "",
                                    Pago = true // Verdadero cuando tiene pago parcial
                                });
                            }
                        }
                    }
                }

                _logger.LogInformation("Payment status check completed for {InvoiceCount} invoices", request.Invoices.Count);

                return new InvoicePaymentStatusListResponse
                {
                    Invoices = responses
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking invoice payment status");
                throw;
            }
        }
    }
}