using Microsoft.Extensions.Configuration;

namespace UmaPay.Service
{
    using Domain;
    using Resource;
    using Interface.Repository;
    using Interface.Service;
    using UmaPay.Interface.Integration.Middleware;

    public class TransactionStatusCommandHandler : ITransactionStatusCommandHandler
    {
        #region Properties

        private readonly ITransactionCommandRepository _transactionCommandRepository;
        private readonly ITransactionQueryRepository _transactionQueryRepository;

        private readonly ITransactionInvoiceCommandRepository _transactionInvoiceCommandRepository;
        private readonly ITransactionInvoiceQueryRepository _transactionInvoiceQueryRepository;

        private readonly IInvoiceService _invoiceService;
        private readonly IConfiguration _configuration;


        private readonly ITransactionStatusLogCommandRepository _transactionStatusLogCommandRepository;
        #endregion Properties

        #region Constructor

        public TransactionStatusCommandHandler(
             ITransactionCommandRepository transactionCommandRepository,
             ITransactionQueryRepository transactionQueryRepository,
             ITransactionInvoiceCommandRepository transactionInvoiceCommandRepository,
             ITransactionInvoiceQueryRepository transactionInvoiceQueryRepository,
             IInvoiceService invoiceService,
             ITransactionStatusLogCommandRepository transactionStatusLogCommandRepository,
             IConfiguration configuration)
        {
            _transactionCommandRepository = transactionCommandRepository ?? throw new ArgumentNullException(nameof(transactionCommandRepository));
            _transactionQueryRepository = transactionQueryRepository ?? throw new ArgumentNullException(nameof(transactionQueryRepository));

            _transactionInvoiceCommandRepository = transactionInvoiceCommandRepository ?? throw new ArgumentNullException(nameof(transactionInvoiceCommandRepository));
            _transactionInvoiceQueryRepository = transactionInvoiceQueryRepository ?? throw new ArgumentNullException(nameof(transactionInvoiceQueryRepository));

            _invoiceService = invoiceService ?? throw new ArgumentNullException(nameof(invoiceService));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            _transactionStatusLogCommandRepository = transactionStatusLogCommandRepository ?? throw new ArgumentNullException(nameof(transactionStatusLogCommandRepository));

        }
        #endregion Constructor

        #region Public Methods

        public async Task<OperationResult<bool>> ProcessFailedInSap()
        {
            try
            {
                var transactions = await _transactionQueryRepository.GetTransactionsByStatusAsync(ConstStatus.Completed);

                if (transactions == null)
                {
                    return OperationResult<bool>.Failure(Message.TransactionNoFound);
                }

                foreach (var transaction in transactions)
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
                                invoice.Status = new TransactionStatus { Id = ConstStatus.CompletedInSap, Name = ConstStatus.GetStatusName(ConstStatus.CompletedInSap) };
                            }
                            else
                            {
                                invoice.Status = new TransactionStatus { Id = ConstStatus.FailedInSap, Name = ConstStatus.GetStatusName(ConstStatus.FailedInSap) };
                            }
                            await _transactionInvoiceCommandRepository.UpdateAsync(transaction.Id, invoice);
                        }

                        //valida si todas las facturas estan pagas
                        var validate = await _transactionInvoiceQueryRepository.IsValidateStatusAsync(transaction.Id, ConstStatus.CompletedInSap);
                        if (validate)
                        {
                            transaction.SapDate = DateTime.UtcNow;
                            transaction.SapDocument = invoiceUpdateResult.Data!.DocumentNumber!;
                            transaction.Status = new TransactionStatus { Id = ConstStatus.CompletedInSap, Name = ConstStatus.GetStatusName(ConstStatus.CompletedInSap) };
                            transaction.SapResponse = invoiceUpdateResult.Data.ResponseContent;
                        }
                        else
                            transaction.Status = new TransactionStatus { Id = ConstStatus.FailedInSap, Name = ConstStatus.GetStatusName(ConstStatus.FailedInSap) };

                        await _transactionCommandRepository.UpdateAsync(transaction);

                        //Adciona Log
                        TransactionStatusLog log = new TransactionStatusLog
                        {
                            Transaction = transaction,
                            Status = transaction.Status,
                            CreatedAt = DateTime.UtcNow,
                            Comment = invoiceUpdateResult.Message
                        };
                        await _transactionStatusLogCommandRepository.AddAsync(log);
                    }
                }
                return OperationResult<bool>.SuccessResult(true);
            }
            catch (Exception ex)
            {
                return OperationResult<bool>.Failure(string.Format(Message.UnexpectedError, ex.Message));
            }
        }

        public async Task<OperationResult<bool>> ProcessFailedInSap(Guid token)
        {
            try
            {
                var transaction = await _transactionQueryRepository.GetTransactionsByStatusAsync(ConstStatus.FailedInSap, token);

                if (transaction == null)
                {
                    return OperationResult<bool>.Failure(Message.TransactionNoFound);
                }

                if (transaction.Status!.Id == ConstStatus.FailedInSap)
                {
                    var invoiceUpdateResult = await _invoiceService.ProcessSapPaymentAsync(
                        transaction.Invoice!, 
                        transaction.Country!.CurrencyCode,
                        transaction.Gateway!.Code);

                    foreach (var invoice in transaction.Invoice!)
                    {
                        if (invoiceUpdateResult.Success)
                        {
                            invoice.Status = new TransactionStatus { Id = ConstStatus.CompletedInSap, Name = ConstStatus.GetStatusName(ConstStatus.CompletedInSap) };
                        }
                        else
                        {
                            invoice.Status = new TransactionStatus { Id = ConstStatus.FailedInSap, Name = ConstStatus.GetStatusName(ConstStatus.FailedInSap) };
                        }
                        await _transactionInvoiceCommandRepository.UpdateAsync(transaction.Id, invoice);
                    }

                    //valida si todas las facturas estan pagas
                    var validate = await _transactionInvoiceQueryRepository.IsValidateStatusAsync(transaction.Id, ConstStatus.CompletedInSap);
                    if (validate)
                    {
                        transaction.SapDate = DateTime.UtcNow;
                        transaction.SapDocument = invoiceUpdateResult.Data!.DocumentNumber!;
                        transaction.Status = new TransactionStatus { Id = ConstStatus.CompletedInSap, Name = ConstStatus.GetStatusName(ConstStatus.CompletedInSap) };
                        transaction.SapResponse = invoiceUpdateResult.Data.ResponseContent;
                    }
                    else
                        transaction.Status = new TransactionStatus { Id = ConstStatus.FailedInSap, Name = ConstStatus.GetStatusName(ConstStatus.FailedInSap) };

                    await _transactionCommandRepository.UpdateAsync(transaction);

                    //Adciona Log
                    TransactionStatusLog log = new TransactionStatusLog
                    {
                        Transaction = transaction,
                        Status = transaction.Status,
                        CreatedAt = DateTime.UtcNow,
                        Comment = invoiceUpdateResult.Message
                    };
                    await _transactionStatusLogCommandRepository.AddAsync(log);

                }

                return OperationResult<bool>.SuccessResult(true);
            }
            catch (Exception ex)
            {
                return OperationResult<bool>.Failure(string.Format(Message.UnexpectedError, ex.Message));
            }
        }

        public async Task<OperationResult<bool>> ProcessFailedInSap(string customer)
        {
            try
            {
                var transactions = await _transactionQueryRepository.GetTransactionsByStatusAsync(ConstStatus.FailedInSap, customer);

                if (transactions == null)
                {
                    return OperationResult<bool>.Failure(Message.TransactionNoFound);
                }

                foreach (var transaction in transactions)
                {
                    if (transaction.Status!.Id == ConstStatus.FailedInSap)
                    {
                        var invoiceUpdateResult = await _invoiceService.ProcessSapPaymentAsync(
                            transaction.Invoice!,
                            transaction.Country!.CurrencyCode,
                            transaction.Gateway!.Code);

                        foreach (var invoice in transaction.Invoice!)
                        {
                            if (invoiceUpdateResult.Success)
                            {
                                invoice.Status = new TransactionStatus { Id = ConstStatus.CompletedInSap, Name = ConstStatus.GetStatusName(ConstStatus.CompletedInSap) };
                            }
                            else
                            {
                                invoice.Status = new TransactionStatus { Id = ConstStatus.FailedInSap, Name = ConstStatus.GetStatusName(ConstStatus.FailedInSap) };
                            }
                            await _transactionInvoiceCommandRepository.UpdateAsync(transaction.Id, invoice);
                        }


                        //valida si todas las facturas estan pagas
                        var validate = await _transactionInvoiceQueryRepository.IsValidateStatusAsync(transaction.Id, ConstStatus.CompletedInSap);
                        if (validate)
                        {
                            transaction.SapDate = DateTime.UtcNow;
                            transaction.SapDocument = invoiceUpdateResult.Data!.DocumentNumber!;
                            transaction.Status = new TransactionStatus { Id = ConstStatus.CompletedInSap, Name = ConstStatus.GetStatusName(ConstStatus.CompletedInSap) };
                            transaction.SapResponse = invoiceUpdateResult.Data.ResponseContent;
                        }
                        else
                            transaction.Status = new TransactionStatus { Id = ConstStatus.FailedInSap, Name = ConstStatus.GetStatusName(ConstStatus.FailedInSap) };

                        await _transactionCommandRepository.UpdateAsync(transaction);

                        //Adciona Log
                        TransactionStatusLog log = new TransactionStatusLog
                        {
                            Transaction = transaction,
                            Status = transaction.Status,
                            CreatedAt = DateTime.UtcNow,
                            Comment = invoiceUpdateResult.Message
                        };
                        await _transactionStatusLogCommandRepository.AddAsync(log);
                    }
                }
                return OperationResult<bool>.SuccessResult(true);
            }
            catch (Exception ex)
            {
                return OperationResult<bool>.Failure(string.Format(Message.UnexpectedError, ex.Message));
            }
        }

        public async Task<OperationResult<bool>> ProcessInitiated()
        {
            try
            {

                #region Status Initiated
                var transactionInitiated = await _transactionQueryRepository.GetTransactionsByStatusAsync(ConstStatus.Initiated);
                if (transactionInitiated != null)
                {
                    var time = int.Parse(_configuration["ScheduledSettings:CleanupToInitiated:Time"]);
                    var cutoffTime = DateTime.UtcNow.AddHours(time * (-1));
                    foreach (var command in transactionInitiated)
                    {
                        if (command.TransactionDate < cutoffTime)
                        {
                            command.Status = new TransactionStatus { Id = ConstStatus.Cancelled, Name = ConstStatus.GetStatusName(ConstStatus.Cancelled) };
                            await _transactionCommandRepository.UpdateAsync(command);

                            //Adciona Log
                            TransactionStatusLog log = new TransactionStatusLog
                            {
                                Transaction = command,
                                Status = command.Status,
                                CreatedAt = DateTime.UtcNow,
                                Comment = Message.TaskProcessInitiated
                            };
                            await _transactionStatusLogCommandRepository.AddAsync(log);


                        }
                    }
                }
                #endregion Status Initiated

                #region Status Initiated
                var transactionProcessing = await _transactionQueryRepository.GetTransactionsByStatusAsync(ConstStatus.Processing);
                if (transactionProcessing != null)
                {
                    foreach (var command in transactionProcessing)
                    {
                        var cutoffTime = DateTime.UtcNow;
                        if (command.ExpirationDate < cutoffTime)
                        {
                            command.Status = new TransactionStatus { Id = ConstStatus.Cancelled, Name = ConstStatus.GetStatusName(ConstStatus.Cancelled) };
                            await _transactionCommandRepository.UpdateAsync(command);

                            //Adciona Log
                            TransactionStatusLog log = new TransactionStatusLog
                            {
                                Transaction = command,
                                Status = command.Status,
                                CreatedAt = DateTime.UtcNow,
                                Comment = Message.TaskProcessInitiated
                            };
                            await _transactionStatusLogCommandRepository.AddAsync(log);


                        }
                    }
                }
                #endregion Status Initiated

                //return OperationResult<bool>.Failure(Message.TransactionNoFound);

                return OperationResult<bool>.SuccessResult(true);
            }
            catch (Exception ex)
            {
                return OperationResult<bool>.Failure(string.Format(Message.UnexpectedError, ex.Message));
            }
        }

        #endregion Public Methods

    }
}