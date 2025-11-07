using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

namespace UmaPay.Repository
{
    using Interface.Repository;
    using Resource;
    using UmaPay.Domain;
    using UmaPay.Repository.Entities;

    public class TransactionInvoiceRepository : GenericRepository<Entities.TransactionInvoice>,
        ITransactionInvoiceQueryRepository,
        ITransactionInvoiceCommandRepository
    {
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public TransactionInvoiceRepository(DataContext context, IMapper mapper, IConfiguration configuration)
            : base(context ?? throw new ArgumentNullException(nameof(context)))
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        #region Query Methods

        public async Task<bool> IsValidateStatusAsync(int transactionId, int statusId)
        {
            var entities = await FindAllAsync(t => t.TransactionId == transactionId && t.StatusId != statusId);
            return !entities.Any();
        }

        public async Task<List<Invoice>> GetLatestPaidInvoicesAsync(List<string> invoiceNumbers)
        {
            var paidStatusIds = new[] { 3, 6, 7 }; // Completed, CompletedInSap, FailedInSap

            var entities = await (from ti in _context.Set<Entities.TransactionInvoice>()
                                 join t in _context.Set<Entities.Transaction>() on ti.TransactionId equals t.TransactionId
                                 where invoiceNumbers.Contains(ti.Number) && paidStatusIds.Contains(t.StatusId)
                                 group ti by ti.Number into g
                                 select g.OrderByDescending(x => x.TransactionInvoiceId).First())
                                 .ToListAsync();

            return _mapper.Map<List<Invoice>>(entities);
        }

        #endregion

        #region Command Methods
        
        public async Task<Invoice> AddAsync(int transactionId, Invoice invoice)
        {
            if (invoice == null)
                throw new ArgumentNullException(nameof(invoice));

            var entity = _mapper.Map<TransactionInvoice>(invoice);
            entity.TransactionId = transactionId;
            entity.StatusId = invoice.Status?.Id ?? throw new InvalidOperationException(Message.Error_InvoiceStatusRequired);

            await AddAsyn(entity);
            return invoice;
        }

        public async Task<Invoice> UpdateAsync(int transactionId, Invoice invoice)
        {
            if (invoice == null)
                throw new ArgumentNullException(nameof(invoice));

            var entity = await FindBy(t => t.TransactionId == transactionId && t.Number == invoice.Number)
                .FirstOrDefaultAsync();

            if (entity == null)
                throw new InvalidOperationException(Message.Error_TransactionInvoiceNotFound);

            entity.StatusId = invoice.Status?.Id ?? throw new InvalidOperationException(Message.Error_InvoiceStatusRequired);
            await UpdateAsync(entity, entity.TransactionInvoiceId);

            return invoice;
        }

        #endregion
    }
}