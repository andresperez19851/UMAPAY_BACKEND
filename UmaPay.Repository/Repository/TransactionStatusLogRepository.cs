using AutoMapper;

namespace UmaPay.Repository
{
    using UmaPay.Domain;
    using Interface.Repository;
    using UmaPay.Resource;

    public class TransactionStatusLogRepository : GenericRepository<Entities.TransactionStatusLog>, ITransactionStatusLogCommandRepository
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;

        public TransactionStatusLogRepository(DataContext context, IMapper mapper) : base(context)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _context = context ?? throw new ArgumentException("Context must be of type DataContext");
        }

        #region Command Methods

        public async Task<TransactionStatusLog> AddAsync(TransactionStatusLog command)
        {
            var entity = _mapper.Map<Entities.TransactionStatusLog>(command);
            entity.TransactionId = command.Transaction!.Id;
            entity.StatusId = command.Status?.Id ?? throw new InvalidOperationException(Message.Error_InvoiceStatusRequired);

            var addedEntity = await AddAsyn(entity);
            command.Id = addedEntity.TransactionStatusLogId;
            return command;
        }

        #endregion
    }
}