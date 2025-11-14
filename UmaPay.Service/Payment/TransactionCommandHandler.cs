using Microsoft.Extensions.Configuration;

namespace UmaPay.Service
{
    using Domain;
    using Resource;
    using Interface.Repository;
    using Interface.Service;
    using UmaPay.Shared;
    using UmaPay.Interface.Shared;
    using UmaPay.Interface.Integration.Middleware;

    public class TransactionCommandHandler : ITransactionCommandHandler
    {
        #region Properties

        private string _baseUrl;

        private readonly ITransactionCommandRepository _transactionCommandRepository;
        private readonly ITransactionQueryRepository _transactionQueryRepository;
        private readonly IGatewayQueryRepository _gatewayQueryRepository;
        private readonly IGatewayServiceFactory _gatewayServiceFactory;
        private readonly ICustomerQueryRepository _customerQueryRepository;
        private readonly ICustomerCommandRepository _customerCommandRepository;
        private readonly ICountryQueryRepository _countryQueryRepository;
        private readonly ITransactionInvoiceCommandRepository _transactionInvoiceCommandRepository;
        private readonly ITransactionStatusLogCommandRepository _transactionStatusLogCommandRepository;
        private readonly ITransactionInvoiceQueryRepository _transactionInvoiceQueryRepository;
        private readonly IMailService _emailService;
        private readonly IInvoiceService _invoiceService;

        #endregion Properties

        #region Constructor

        public TransactionCommandHandler(
             ITransactionCommandRepository transactionCommandRepository,
             ITransactionQueryRepository transactionQueryRepository,
             IGatewayQueryRepository gatewayQueryRepository,
             IGatewayServiceFactory gatewayServiceFactory,
             ICustomerQueryRepository customerQueryRepository,
             ICustomerCommandRepository customerCommandRepository,
             ICountryQueryRepository countryQueryRepository,
             ITransactionInvoiceCommandRepository transactionInvoiceCommandRepository,
             ITransactionStatusLogCommandRepository transactionStatusLogCommandRepository,
             IConfiguration configuration,
             IMailService emailService,
             IInvoiceService invoiceService,
             ITransactionInvoiceQueryRepository transactionInvoiceQueryRepository)
        {
            _transactionCommandRepository = transactionCommandRepository ?? throw new ArgumentNullException(nameof(transactionCommandRepository));
            _transactionQueryRepository = transactionQueryRepository ?? throw new ArgumentNullException(nameof(transactionQueryRepository));
            _gatewayQueryRepository = gatewayQueryRepository ?? throw new ArgumentNullException(nameof(gatewayQueryRepository));
            _gatewayServiceFactory = gatewayServiceFactory ?? throw new ArgumentNullException(nameof(gatewayServiceFactory));
            _customerQueryRepository = customerQueryRepository ?? throw new ArgumentNullException(nameof(customerQueryRepository));
            _customerCommandRepository = customerCommandRepository ?? throw new ArgumentNullException(nameof(customerCommandRepository));
            _countryQueryRepository = countryQueryRepository ?? throw new ArgumentNullException(nameof(countryQueryRepository));
            _transactionInvoiceCommandRepository = transactionInvoiceCommandRepository ?? throw new ArgumentNullException(nameof(transactionInvoiceCommandRepository));
            _transactionStatusLogCommandRepository = transactionStatusLogCommandRepository ?? throw new ArgumentNullException(nameof(transactionStatusLogCommandRepository));
            _baseUrl = configuration["ApiSettings:BaseUrl"] ?? throw new InvalidOperationException("BaseUrl configuration is missing");
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _invoiceService = invoiceService ?? throw new ArgumentNullException(nameof(invoiceService));
            _transactionInvoiceQueryRepository = transactionInvoiceQueryRepository;
        }
        #endregion Constructor

        #region Public Methods

        public async Task<OperationResult<Transaction>> CreateAsync(Transaction command)
        {
            try
            {
                //donde curdar el pais
                command.Country = await _countryQueryRepository.GetByNameAsync(command.Country!.Name);
                if (command.Country == null)
                {
                    return OperationResult<Transaction>.Failure(Message.CountryNameInvalid);
                }

                // Verificar y crear cliente si no existe
                var customerResult = await VerifyAndCreateCustomerAsync(command.Customer!);
                if (!customerResult.Success)
                {
                    return OperationResult<Transaction>.Failure(customerResult.Message);
                }

                //var gatewayResult = await _gatewayQueryRepository.GetByApplicationAsync()


                command.Amount = command.Invoice?.Sum(s => s.Amount) ?? 0;
                if (command.Amount <= 0)
                {
                    return OperationResult<Transaction>.Failure(Message.PaymentAmountInvalid);
                }

                command.Customer = customerResult.Data;
                command.Status = new TransactionStatus { Id = ConstStatus.Initiated, Name = ConstStatus.GetStatusName(ConstStatus.Initiated) };
                command.TransactionDate = DateTime.UtcNow;
                command.Description = BuildTransactionDescription(command.Invoice!, 100);

                // Almacena transacción
                command = await _transactionCommandRepository.AddAsync(command);

                //Almacena Factura
                foreach (var invoice in command.Invoice!)
                {
                    invoice.Status = new TransactionStatus { Id = ConstStatus.Initiated, Name = ConstStatus.GetStatusName(ConstStatus.Initiated) };
                    await _transactionInvoiceCommandRepository.AddAsync(command.Id, invoice);
                }

                //Adciona Log
                TransactionStatusLog log = new TransactionStatusLog
                {
                    Transaction = command,
                    Status = new TransactionStatus { Id = ConstStatus.Initiated, Name = ConstStatus.GetStatusName(ConstStatus.Initiated) },
                    CreatedAt = DateTime.UtcNow
                };
                await _transactionStatusLogCommandRepository.AddAsync(log);

                return OperationResult<Transaction>.SuccessResult(command);

            }
            catch (Exception ex)
            {
                return OperationResult<Transaction>.Failure(string.Format(Message.UnexpectedError, ex.Message));
            }
        }

        public async Task<OperationResult<Transaction>> GenereLinkAsync(Guid tokenCommand, Gateway gatewayCommand, string email)
        {
            try
            {
                //Consulta la transaccion
                var transaction = await _transactionQueryRepository.GetByTokenSingleAsync(tokenCommand);
                transaction.Customer.Email = email;

                //actualiza correo del cliente
                await _customerCommandRepository.UpdateAsync(transaction.Customer);

                // Valida información pasarela
                var gateway = await _gatewayQueryRepository.GetByApplicationAsync(gatewayCommand.Id, gatewayCommand!.Code, transaction.Country.CurrencyCode);
                if (gateway == null)
                {
                    return OperationResult<Transaction>.Failure(Message.GatewayNotFound);
                }

                transaction.Amount = transaction.Invoice?.Sum(s => s.Amount) ?? 0;
                if (transaction.Amount <= 0)
                {
                    return OperationResult<Transaction>.Failure(Message.PaymentAmountInvalid);
                }

                // Recupera la pasarela de pagos
                var gatewayService = _gatewayServiceFactory.GetGatewayService(gateway.Code ?? throw new ArgumentException(Message.GatewayRequired));

                transaction.Gateway = gateway;
                transaction.Description = BuildTransactionDescription(transaction.Invoice, 100);
                transaction.Url = new TransactionUrl
                {
                    SuccessUrl = $"{_baseUrl}sucess/{transaction.Token}",
                    FailureUrl = $"{_baseUrl}failure/{transaction.Token}",
                    PendingUrl = $"{_baseUrl}pending/{transaction.Token}",
                    ReviewUrl = $"{_baseUrl}review/{transaction.Token}",
                };

                // Invoca pago
                var gatewayResponse = await gatewayService.GeneratePaymentLinkAsync(transaction);

                //recupera información de pasarela
                transaction.GatewayRequest = gatewayResponse.Data?.GatewayRequest!;
                transaction.GatewayResponse = gatewayResponse.Data?.GatewayResponse!;

                if (gatewayResponse.Success)
                {
                    transaction.Status = new TransactionStatus { Id = ConstStatus.Processing, Name = ConstStatus.GetStatusName(ConstStatus.Processing) };
                    transaction.Reference = gatewayResponse.Data!.Reference;
                    transaction.ExpirationDate = gatewayResponse.Data.ExpirationDate;
                    transaction.PaymentUrl = gatewayResponse.Data!.Url!.PaymentUrl;
                }
                else
                {
                    transaction.Status = new TransactionStatus { Id = ConstStatus.Failed, Name = ConstStatus.GetStatusName(ConstStatus.Failed) };
                }

                //actualiza transaccion
                transaction = await _transactionCommandRepository.UpdateAsync(transaction);

                //Adciona Log
                TransactionStatusLog log = new TransactionStatusLog
                {
                    Transaction = transaction,
                    Status = transaction.Status,
                    CreatedAt = DateTime.UtcNow
                };
                await _transactionStatusLogCommandRepository.AddAsync(log);


                return gatewayResponse.Success
                    ? OperationResult<Transaction>.SuccessResult(transaction)
                    : OperationResult<Transaction>.Failure(gatewayResponse.Message ?? Message.UnknownError, transaction);
            }
            catch (Exception ex)
            {
                return OperationResult<Transaction>.Failure(string.Format(Message.UnexpectedError, ex.Message));
            }
        }

        public async Task ProcessTransactionInvoices(Transaction transaction)
        {
            if (transaction.Status!.Id == ConstStatus.Completed)
            {
                var invoiceUpdateResult = await _invoiceService.ProcessSapPaymentAsync(
                    transaction.Invoice!,
                    transaction.Country!.CurrencyCode,
                    transaction.Gateway!.Code);

                foreach (var invoice in transaction.Invoice!)
                {
                    if (invoiceUpdateResult.Success)
                    {
                        invoice.Status = new TransactionStatus
                        {
                            Id = ConstStatus.CompletedInSap,
                            Name = ConstStatus.GetStatusName(ConstStatus.CompletedInSap)
                        };
                    }
                    else
                    {
                        invoice.Status = new TransactionStatus
                        {
                            Id = ConstStatus.FailedInSap,
                            Name = ConstStatus.GetStatusName(ConstStatus.FailedInSap)
                        };
                    }

                    await _transactionInvoiceCommandRepository.UpdateAsync(transaction.Id, invoice);
                }

                //valida si todas las facturas estan pagas
                var areInvoicesFullyPaid = await _transactionInvoiceQueryRepository.IsValidateStatusAsync(
                    transaction.Id,
                    ConstStatus.CompletedInSap);

                if (areInvoicesFullyPaid)
                {
                    transaction.SapDate = DateTime.UtcNow;
                    transaction.SapDocument = invoiceUpdateResult.Data!.DocumentNumber!;
                    transaction.SapRequest = invoiceUpdateResult.Data!.RequestContent;
                    transaction.SapResponse = invoiceUpdateResult.Data!.ResponseContent;
                    transaction.Status = new TransactionStatus
                    {
                        Id = ConstStatus.CompletedInSap,
                        Name = ConstStatus.GetStatusName(ConstStatus.CompletedInSap)
                    };
                }
                else
                {
                    transaction.SapRequest = invoiceUpdateResult.Data?.RequestContent;
                    transaction.SapResponse = invoiceUpdateResult.Data!.ResponseContent;
                    transaction.Status = new TransactionStatus
                    {
                        Id = ConstStatus.FailedInSap,
                        Name = ConstStatus.GetStatusName(ConstStatus.FailedInSap)
                    };
                }

                transaction = await _transactionCommandRepository.UpdateAsync(transaction);

                TransactionStatusLog log = new()
                {
                    Transaction = transaction,
                    Status = transaction.Status,
                    CreatedAt = DateTime.UtcNow,
                    Comment = invoiceUpdateResult.Message
                };

                await _transactionStatusLogCommandRepository.AddAsync(log);

                //envio de correo
                var invoices = transaction.Invoice!.Select(i => (i.Number, i.Amount)).ToList();
                var emailBody = PaymentNotificationTemplate.GetTemplate(
                    reference: transaction.Reference,
                    customerName: string.Format("{0} {1}", transaction.Customer.FirstName, transaction.Customer.LastName),
                    invoices: invoices,
                    totalAmount: transaction.Amount,
                    paymentDate: DateTime.Now);

                await _emailService.SendAsync(transaction.Customer.Email, PaymentNotificationTemplate.Subject, emailBody);
            }
        }


        #endregion Public Methods

        #region Private Methods

        private async Task<OperationResult<Customer>> VerifyAndCreateCustomerAsync(Customer customer)
        {
            if (customer == null)
            {
                return OperationResult<Customer>.Failure(Message.CustomerRequired);
            }

            if (string.IsNullOrWhiteSpace(customer.Email))
            {
                return OperationResult<Customer>.Failure(Message.CustomerEmailRequired);
            }

            if (string.IsNullOrWhiteSpace(customer.CodeSap))
            {
                return OperationResult<Customer>.Failure(Message.CustomerCodeSapRequired);
            }

            if (string.IsNullOrWhiteSpace(customer.FirstName))
            {
                return OperationResult<Customer>.Failure(Message.CustomerFirstNameRequired);
            }

            //Se deshabilita validación de apellido ya que representante legal viene sin apellido desde Umaone.
            //if (string.IsNullOrWhiteSpace(customer.LastName))
            //{
            //    return OperationResult<Customer>.Failure(Message.CustomerLastNameRequired);
            //}

            try
            {
                //var existingCustomer = await _customerQueryRepository.GetByCodeSapAsync(customer.CodeSap);
                //if (existingCustomer != null)
                //{
                //    // Actualizar información del cliente si es necesario
                //    existingCustomer.FirstName = customer.FirstName;
                //    existingCustomer.LastName = customer.LastName;
                //    existingCustomer = await _customerCommandRepository.UpdateAsync(existingCustomer);
                //    return OperationResult<Customer>.SuccessResult(existingCustomer);
                //}
                //else
                //{
                // Crear nuevo cliente
                var newCustomer = await _customerCommandRepository.AddAsync(customer);
                return OperationResult<Customer>.SuccessResult(newCustomer);
                //}
            }
            catch (Exception ex)
            {
                // Log the exception here
                return OperationResult<Customer>.Failure(string.Format(Message.CustomerCreationError, ex.Message));
            }
        }

        private string BuildTransactionDescription(ICollection<Invoice>? invoices, int maxLength)
        {
            if (invoices is null || invoices.Count == 0)
            {
                return string.Empty;
            }

            var invoiceNumbers = invoices
                .Where(invoice => !string.IsNullOrWhiteSpace(invoice.Number))
                .Select(invoice => invoice.Number)
                .ToArray();

            if (invoiceNumbers.Length == 0)
            {
                return string.Empty;
            }

            string description = string.Join(", ", invoiceNumbers);
            return description.Length > maxLength ? description[..maxLength] : description;
        }


        #endregion Private Methods
    }
}