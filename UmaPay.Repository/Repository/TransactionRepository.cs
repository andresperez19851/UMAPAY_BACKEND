using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace UmaPay.Repository
{
    using Interface.Repository;
    using Domain;

    public class TransactionRepository : GenericRepository<Entities.Transaction>, ITransactionQueryRepository, ITransactionCommandRepository
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;

        public TransactionRepository(DataContext context, IMapper mapper) : base(context)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _context = context ?? throw new ArgumentException("Context must be of type DataContext");
        }

        #region Query Methods

        public async Task<Transaction> GetByIdAsync(int id)
        {
            var entity = await (from t in _context.Transactions
                                join c in _context.Customers on t.CustomerId equals c.CustormerId
                                join ts in _context.TransactionStatuses on t.StatusId equals ts.StatusId
                                join co in _context.Countries on t.CountryId equals co.CountryId
                                join ga in _context.GatewayApplications on t.GatewayApplicationId equals ga.GatewayApplicationId into gatewayApps
                                from ga in gatewayApps.DefaultIfEmpty()
                                join g in _context.Gateways on ga.GatewayId equals g.GatewayId into gateways
                                from g in gateways.DefaultIfEmpty()
                                where t.TransactionId == id
                                select new Transaction
                                {
                                    Id = t.TransactionId,
                                    Token = t.Token,
                                    Reference = t.Reference,
                                    Amount = t.Amount,
                                    ExpirationDate = t.ExpirationDate,
                                    SapDate = t.SapDate,
                                    SapDocument = t.SapDocument,
                                    SapRequest = t.SapRequest,
                                    SapResponse = t.SapResponse,
                                    PaymentUrl = t.PaymentUrl,
                                    GatewayPayment = t.GatewayPayment,
                                    GatewayRequest = t.GatewayRequest,
                                    GatewayResponse = t.GatewayResponse,
                                    TransactionDate = t.TransactionDate,
                                    Customer = new Customer
                                    {
                                        Id = c.CustormerId,
                                        FirstName = c.FirstName,
                                        CodeSap = c.CodeSap,
                                        LastName = c.LastName,
                                        Society = c.Society,
                                        User = c.User,
                                        Email = c.Email
                                    },
                                    Gateway = g == null ? null : new Gateway { Id = g.GatewayId, Code = g.Code, Name = g.Name },
                                    Status = new TransactionStatus { Id = ts.StatusId, Name = ts.Name, Description = ts.Description },
                                    Invoice = (from invoices in _context.TransactionInvoices
                                               join statusinvoce in _context.TransactionStatuses on invoices.StatusId equals statusinvoce.StatusId
                                               where invoices.TransactionId == t.TransactionId
                                               select new Invoice
                                               {
                                                   Amount = invoices.Amount,
                                                   Number = invoices.Number,
                                                   Status = new TransactionStatus
                                                   {
                                                       Id = statusinvoce.StatusId,
                                                       Name = statusinvoce.Name,
                                                       Description = statusinvoce.Description
                                                   }
                                               }).ToList(),
                                    Country = new Country
                                    {
                                        Id = co.CountryId,
                                        CurrencyCode = co.CurrencyCode,
                                        CurrencyName = co.CurrencyName,
                                        Name = co.Name
                                    }
                                })
                                .FirstOrDefaultAsync();
            return entity!;
        }

        public async Task<IEnumerable<Transaction>> GetByStatusAsync(int statusId)
        {
            var entities = await FindAllAsync(t => t.StatusId == statusId);
            return _mapper.Map<IEnumerable<Transaction>>(entities);
        }

        public async Task<IEnumerable<Transaction>> GetAllAsync()
        {
            var entities = await GetAllAsyn();
            return _mapper.Map<IEnumerable<Transaction>>(entities);
        }

        public async Task<Transaction> GetByReferenceAndGatewayAsync(string reference, string gatewayCode)
        {
            var entity = await (from t in _context.Transactions
                                join ga in _context.GatewayApplications on t.GatewayApplicationId equals ga.GatewayApplicationId
                                join g in _context.Gateways on ga.GatewayId equals g.GatewayId
                                join c in _context.Customers on t.CustomerId equals c.CustormerId
                                join ts in _context.TransactionStatuses on t.StatusId equals ts.StatusId
                                join co in _context.Countries on t.CountryId equals co.CountryId
                                where t.TransactionId == int.Parse(reference) && g.Code == gatewayCode
                                select new Transaction
                                {
                                    Amount = t.Amount,
                                    ExpirationDate = t.ExpirationDate,
                                    Id = t.TransactionId,
                                    Token = t.Token,
                                    Reference = t.Reference,
                                    SapDate = t.SapDate,
                                    PaymentUrl = t.PaymentUrl,
                                    GatewayPayment = t.GatewayPayment,
                                    GatewayRequest = t.GatewayRequest,
                                    GatewayResponse = t.GatewayResponse,
                                    TransactionDate = t.TransactionDate,
                                    Country = new Country { Id = co.CountryId, CurrencyCode = co.CurrencyCode, CurrencyName = co.CurrencyName, Name = co.Name },
                                    Customer = new Customer { Id = c.CustormerId, FirstName = c.FirstName, CodeSap = c.CodeSap, LastName = c.LastName, Society = c.Society, User = c.User, Email = c.Email },
                                    Gateway = new Gateway { Id = g.GatewayId, Code = g.Code, Name = g.Name },
                                    Status = new TransactionStatus { Id = ts.StatusId, Name = ts.Name },
                                    Invoice = (from invoices in _context.TransactionInvoices
                                               join statusinvoce in _context.TransactionStatuses on invoices.StatusId equals statusinvoce.StatusId
                                               where invoices.TransactionId == t.TransactionId
                                               select new Invoice { Amount = invoices.Amount, Number = invoices.Number, Status = new TransactionStatus { Id = statusinvoce.StatusId, Name = statusinvoce.Name, Description = statusinvoce.Description } }).ToList(),
                                })
                                .FirstOrDefaultAsync();

            return entity;
        }

        public async Task<Transaction?> GetByGatewayReference(string gatewayId, string gatewayCode)
        {
            return await (from transaction in _context.Transactions
                          join ga in _context.GatewayApplications on transaction.GatewayApplicationId equals ga.GatewayApplicationId
                          join g in _context.Gateways on ga.GatewayId equals g.GatewayId
                          join c in _context.Customers on transaction.CustomerId equals c.CustormerId
                          join ts in _context.TransactionStatuses on transaction.StatusId equals ts.StatusId
                          join co in _context.Countries on transaction.CountryId equals co.CountryId
                          where transaction.Reference == gatewayId && g.Code == gatewayCode
                          select new Transaction
                          {
                              Amount = transaction.Amount,
                              ExpirationDate = transaction.ExpirationDate,
                              Id = transaction.TransactionId,
                              Token = transaction.Token,
                              Reference = transaction.Reference,
                              SapDate = transaction.SapDate,
                              PaymentUrl = transaction.PaymentUrl,
                              GatewayPayment = transaction.GatewayPayment,
                              GatewayRequest = transaction.GatewayRequest,
                              GatewayResponse = transaction.GatewayResponse,
                              TransactionDate = transaction.TransactionDate,
                              Country = new Country { Id = co.CountryId, CurrencyCode = co.CurrencyCode, CurrencyName = co.CurrencyName, Name = co.Name },
                              Customer = new Customer { Id = c.CustormerId, FirstName = c.FirstName, CodeSap = c.CodeSap, LastName = c.LastName, Society = c.Society, User = c.User, Email = c.Email },
                              Gateway = new Gateway { Id = g.GatewayId, Code = g.Code, Name = g.Name },
                              Status = new TransactionStatus { Id = ts.StatusId, Name = ts.Name },
                              Invoice = (from invoices in _context.TransactionInvoices
                                         join statusinvoce in _context.TransactionStatuses on invoices.StatusId equals statusinvoce.StatusId
                                         where invoices.TransactionId == transaction.TransactionId
                                         select new Invoice { Amount = invoices.Amount, Number = invoices.Number, Status = new TransactionStatus { Id = statusinvoce.StatusId, Name = statusinvoce.Name, Description = statusinvoce.Description } }).ToList(),
                          }).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsByStatusAsync(int status)
        {
            var entities = await (from t in _context.Transactions
                                  join c in _context.Customers on t.CustomerId equals c.CustormerId
                                  join ts in _context.TransactionStatuses on t.StatusId equals ts.StatusId
                                  join co in _context.Countries on t.CountryId equals co.CountryId
                                  join ga in _context.GatewayApplications on t.GatewayApplicationId equals ga.GatewayApplicationId into gatewayApps
                                  from ga in gatewayApps.DefaultIfEmpty()
                                  join g in _context.Gateways on ga.GatewayId equals g.GatewayId into gateways
                                  from g in gateways.DefaultIfEmpty()
                                  where t.StatusId == status
                                  select new Transaction
                                  {
                                      Id = t.TransactionId,
                                      Token = t.Token,
                                      Reference = t.Reference,
                                      Amount = t.Amount,
                                      ExpirationDate = t.ExpirationDate,
                                      SapDate = t.SapDate,
                                      PaymentUrl = t.PaymentUrl,
                                      GatewayPayment = t.GatewayPayment,
                                      GatewayRequest = t.GatewayRequest,
                                      GatewayResponse = t.GatewayResponse,
                                      TransactionDate = t.TransactionDate,
                                      Customer = new Customer { Id = c.CustormerId, FirstName = c.FirstName, CodeSap = c.CodeSap, LastName = c.LastName, Society = c.Society, User = c.User, Email = c.Email },
                                      Gateway = g == null ? null : new Gateway { Id = g.GatewayId, Code = g.Code, Name = g.Name },
                                      Status = new TransactionStatus { Id = ts.StatusId, Name = ts.Name, Description = ts.Description },
                                      Invoice = _context.TransactionInvoices.Where(i => i.TransactionId == t.TransactionId).Select(i => new Invoice { Amount = i.Amount, Number = i.Number }).ToList(),
                                      Country = new Country { Id = co.CountryId, CurrencyCode = co.CurrencyCode, CurrencyName = co.CurrencyName, Name = co.Name }
                                  })
                                  .ToListAsync();
            return entities;
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsByStatusAsync(int status, string customer)
        {
            var entities = await (from t in _context.Transactions
                                  join c in _context.Customers on t.CustomerId equals c.CustormerId
                                  join ts in _context.TransactionStatuses on t.StatusId equals ts.StatusId
                                  join co in _context.Countries on t.CountryId equals co.CountryId
                                  join ga in _context.GatewayApplications on t.GatewayApplicationId equals ga.GatewayApplicationId into gatewayApps
                                  from ga in gatewayApps.DefaultIfEmpty()
                                  join g in _context.Gateways on ga.GatewayId equals g.GatewayId into gateways
                                  from g in gateways.DefaultIfEmpty()
                                  where t.StatusId == status && c.CodeSap == customer
                                  select new Transaction
                                  {
                                      Id = t.TransactionId,
                                      Token = t.Token,
                                      Reference = t.Reference,
                                      Amount = t.Amount,
                                      ExpirationDate = t.ExpirationDate,
                                      SapDate = t.SapDate,
                                      PaymentUrl = t.PaymentUrl,
                                      GatewayPayment = t.GatewayPayment,
                                      GatewayRequest = t.GatewayRequest,
                                      GatewayResponse = t.GatewayResponse,
                                      TransactionDate = t.TransactionDate,
                                      Customer = new Customer { Id = c.CustormerId, FirstName = c.FirstName, CodeSap = c.CodeSap, LastName = c.LastName, Society = c.Society, User = c.User, Email = c.Email },
                                      Gateway = g == null ? null : new Gateway { Id = g.GatewayId, Code = g.Code, Name = g.Name },
                                      Status = new TransactionStatus { Id = ts.StatusId, Name = ts.Name, Description = ts.Description },
                                      Invoice = _context.TransactionInvoices.Where(i => i.TransactionId == t.TransactionId).Select(i => new Invoice { Amount = i.Amount, Number = i.Number }).ToList(),
                                      Country = new Country { Id = co.CountryId, CurrencyCode = co.CurrencyCode, CurrencyName = co.CurrencyName, Name = co.Name }
                                  })
                                  .ToListAsync();
            return entities;
        }

        public async Task<Transaction> GetTransactionsByStatusAsync(int status, Guid token)
        {
            var entities = await (from t in _context.Transactions
                                  join c in _context.Customers on t.CustomerId equals c.CustormerId
                                  join ts in _context.TransactionStatuses on t.StatusId equals ts.StatusId
                                  join co in _context.Countries on t.CountryId equals co.CountryId
                                  join ga in _context.GatewayApplications on t.GatewayApplicationId equals ga.GatewayApplicationId into gatewayApps
                                  from ga in gatewayApps.DefaultIfEmpty()
                                  join g in _context.Gateways on ga.GatewayId equals g.GatewayId into gateways
                                  from g in gateways.DefaultIfEmpty()
                                  where t.StatusId == status
                                  && t.Token == token
                                  select new Transaction
                                  {
                                      Id = t.TransactionId,
                                      Token = t.Token,
                                      Reference = t.Reference,
                                      Amount = t.Amount,
                                      ExpirationDate = t.ExpirationDate,
                                      SapDate = t.SapDate,
                                      PaymentUrl = t.PaymentUrl,
                                      GatewayPayment = t.GatewayPayment,
                                      GatewayRequest = t.GatewayRequest,
                                      GatewayResponse = t.GatewayResponse,
                                      TransactionDate = t.TransactionDate,
                                      Customer = new Customer { Id = c.CustormerId, FirstName = c.FirstName, CodeSap = c.CodeSap, LastName = c.LastName, Society = c.Society, User = c.User, Email = c.Email },
                                      Gateway = g == null ? null : new Gateway { Id = g.GatewayId, Code = g.Code, Name = g.Name },
                                      Status = new TransactionStatus { Id = ts.StatusId, Name = ts.Name, Description = ts.Description },
                                      Invoice = _context.TransactionInvoices.Where(i => i.TransactionId == t.TransactionId).Select(i => new Invoice { Amount = i.Amount, Number = i.Number }).ToList(),
                                      Country = new Country { Id = co.CountryId, CurrencyCode = co.CurrencyCode, CurrencyName = co.CurrencyName, Name = co.Name }
                                  })
                                  .FirstOrDefaultAsync();
            return entities;
        }

        public async Task<Transaction> GetByTokenAsync(Guid token)
        {
            var entity = await (from t in _context.Transactions
                                join c in _context.Customers on t.CustomerId equals c.CustormerId
                                join ts in _context.TransactionStatuses on t.StatusId equals ts.StatusId
                                join co in _context.Countries on t.CountryId equals co.CountryId
                                join ga in _context.GatewayApplications on t.GatewayApplicationId equals ga.GatewayApplicationId into gatewayApps
                                from ga in gatewayApps.DefaultIfEmpty()
                                join g in _context.Gateways on ga.GatewayId equals g.GatewayId into gateways
                                from g in gateways.DefaultIfEmpty()
                                where t.Token == token
                                select new Transaction
                                {
                                    Id = t.TransactionId,
                                    Token = t.Token,
                                    Reference = t.Reference,
                                    Amount = t.Amount,
                                    ExpirationDate = t.ExpirationDate,
                                    SapDate = t.SapDate,
                                    PaymentUrl = t.PaymentUrl,
                                    GatewayPayment = t.GatewayPayment,
                                    GatewayRequest = t.GatewayRequest,
                                    GatewayResponse = t.GatewayResponse,
                                    TransactionDate = t.TransactionDate,
                                    Customer = new Customer { Id = c.CustormerId, FirstName = c.FirstName, CodeSap = c.CodeSap, LastName = c.LastName, Society = c.Society, User = c.User, Email = c.Email },
                                    Gateway = g == null ? null : new Gateway { Id = g.GatewayId, Code = g.Code, Name = g.Name },
                                    Status = new TransactionStatus { Id = ts.StatusId, Name = ts.Name, Description = ts.Description },
                                    Invoice = _context.TransactionInvoices.Where(i => i.TransactionId == t.TransactionId).Select(i => new Invoice { Amount = i.Amount, Number = i.Number }).ToList(),
                                    Country = new Country { Id = co.CountryId, CurrencyCode = co.CurrencyCode, CurrencyName = co.CurrencyName, Name = co.Name }
                                })
                          .FirstOrDefaultAsync();
            return entity!;
        }

        public async Task<Transaction> GetByTokenCompleteAsync(Guid token)
        {
            var entity = await (from t in _context.Transactions
                                join c in _context.Customers on t.CustomerId equals c.CustormerId
                                join ts in _context.TransactionStatuses on t.StatusId equals ts.StatusId
                                join co in _context.Countries on t.CountryId equals co.CountryId
                                join ga in _context.GatewayApplications on t.GatewayApplicationId equals ga.GatewayApplicationId into gatewayApps
                                from ga in gatewayApps.DefaultIfEmpty()
                                join g in _context.Gateways on ga.GatewayId equals g.GatewayId into gateways
                                from g in gateways.DefaultIfEmpty()
                                where t.Token == token
                                select new Transaction
                                {
                                    Id = t.TransactionId,
                                    Token = t.Token,
                                    Reference = t.Reference,
                                    Amount = t.Amount,
                                    ExpirationDate = t.ExpirationDate,
                                    SapDate = t.SapDate,
                                    SapDocument = t.SapDocument,
                                    SapRequest = t.SapRequest,
                                    SapResponse = t.SapResponse,
                                    PaymentUrl = t.PaymentUrl,
                                    GatewayPayment = t.GatewayPayment,
                                    GatewayRequest = t.GatewayRequest,
                                    GatewayResponse = t.GatewayResponse,
                                    TransactionDate = t.TransactionDate,
                                    Customer = new Customer
                                    {
                                        Id = c.CustormerId,
                                        FirstName = c.FirstName,
                                        CodeSap = c.CodeSap,
                                        LastName = c.LastName,
                                        Society = c.Society,
                                        User = c.User,
                                        Email = c.Email
                                    },
                                    Gateway = g == null ? null : new Gateway { Id = g.GatewayId, Code = g.Code, Name = g.Name },
                                    Status = new TransactionStatus { Id = ts.StatusId, Name = ts.Name, Description = ts.Description },
                                    Invoice = (from invoices in _context.TransactionInvoices
                                               join statusinvoce in _context.TransactionStatuses on invoices.StatusId equals statusinvoce.StatusId
                                               where invoices.TransactionId == t.TransactionId
                                               select new Invoice
                                               {
                                                   Amount = invoices.Amount,
                                                   Number = invoices.Number,
                                                   Status = new TransactionStatus
                                                   {
                                                       Id = statusinvoce.StatusId,
                                                       Name = statusinvoce.Name,
                                                       Description = statusinvoce.Description
                                                   }
                                               }).ToList(),
                                    Country = new Country
                                    {
                                        Id = co.CountryId,
                                        CurrencyCode = co.CurrencyCode,
                                        CurrencyName = co.CurrencyName,
                                        Name = co.Name
                                    }
                                })
                                .FirstOrDefaultAsync();
            return entity!;
        }

        public async Task<Transaction> GetByTokenSingleAsync(Guid token)
        {
            var entity = await (from t in _context.Transactions
                                join c in _context.Customers on t.CustomerId equals c.CustormerId
                                join ts in _context.TransactionStatuses on t.StatusId equals ts.StatusId
                                join co in _context.Countries on t.CountryId equals co.CountryId
                                where t.Token == token
                                select new Transaction
                                {
                                    Id = t.TransactionId,
                                    Token = t.Token,
                                    Amount = t.Amount,
                                    Reference = t.Reference ?? string.Empty,
                                    TransactionDate = t.TransactionDate,
                                    Customer = new Customer { Id = c.CustormerId, FirstName = c.FirstName, CodeSap = c.CodeSap, LastName = c.LastName, Society = c.Society, User = c.User, Email = c.Email },
                                    Status = new TransactionStatus { Id = ts.StatusId, Name = ts.Name, Description = ts.Description },
                                    Invoice = (from invoices in _context.TransactionInvoices
                                               join statusinvoce in _context.TransactionStatuses on invoices.StatusId equals statusinvoce.StatusId
                                               where invoices.TransactionId == t.TransactionId
                                               select new Invoice
                                               {
                                                   Amount = invoices.Amount,
                                                   Number = invoices.Number,
                                                   Status = new TransactionStatus
                                                   {
                                                       Id = statusinvoce.StatusId,
                                                       Name = statusinvoce.Name,
                                                       Description = statusinvoce.Description
                                                   }
                                               }
                                               ).ToList(),
                                    Country = new Country { Id = co.CountryId, CurrencyCode = co.CurrencyCode, CurrencyName = co.CurrencyName, Name = co.Name },
                                    Gateway = t.GatewayApplicationId == null ? null : (from g in _context.Gateways
                                                                                       join ga in _context.GatewayApplications on g.GatewayId equals ga.GatewayId
                                                                                       where ga.GatewayApplicationId == t.GatewayApplicationId
                                                                                       select new Gateway { Id = ga.GatewayApplicationId, Code = g.Code, Name = g.Name }).FirstOrDefault()
                                })
                                .FirstOrDefaultAsync();
            return entity!;
        }

        public async Task<IEnumerable<TransactionReport>> SearchTransactionsAsync(DateTime? startDate, DateTime? endDate, string? status, Guid? token, string customer)
        {
            var query = from t in _context.Transactions
                        join c in _context.Customers on t.CustomerId equals c.CustormerId
                        join ts in _context.TransactionStatuses on t.StatusId equals ts.StatusId
                        join i in _context.TransactionInvoices on t.TransactionId equals i.TransactionId
                        join co in _context.Countries on t.CountryId equals co.CountryId
                        join ga in _context.GatewayApplications on t.GatewayApplicationId equals ga.GatewayApplicationId into gatewayApps
                        from ga in gatewayApps.DefaultIfEmpty()
                        join g in _context.Gateways on ga.GatewayId equals g.GatewayId into gateways
                        from g in gateways.DefaultIfEmpty()
                        select new TransactionReport
                        {
                            Id = t.TransactionId,
                            AmountBilling = i.Amount,
                            AmountGateway = t.Amount,
                            Token = t.Token,
                            Reference = t.Reference,
                            TransactionDate = t.TransactionDate,
                            StatusName = ts.Name,
                            Status = ts.Description,
                            Gateway = g.Name,
                            Billing = i.Number,
                            SapDate = t.SapDate,
                            CodeSap = c.CodeSap,
                            DocumentSap = t.SapDocument
                        };

            if (startDate.HasValue)
                query = query.Where(t => t.TransactionDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(t => t.TransactionDate <= endDate.Value);

            if (!string.IsNullOrEmpty(status))
                query = query.Where(t => t.StatusName == status);

            if (token.HasValue)
                query = query.Where(t => t.Token == token.Value);


            if (!string.IsNullOrEmpty(customer))
                query = query.Where(t => t.CodeSap == customer);

            return await query.ToListAsync();
        }

        public async Task<bool> IsTokenUniqueAsync(Guid token)
        {
            return !await _context.Transactions.AnyAsync(t => t.Token == token);
        }

        #endregion

        #region Command Methods

        public async Task<Transaction> AddAsync(Transaction command)
        {
            var entity = _mapper.Map<Entities.Transaction>(command);
            entity.StatusId = command.Status!.Id;
            entity.CustomerId = command.Customer!.Id;
            entity.CountryId = command.Country!.Id;

            if (command.Gateway != null)
                entity.GatewayApplicationId = command.Gateway.Id;

            var addedEntity = await AddAsyn(entity);
            command.Id = addedEntity.TransactionId;
            return command;
        }

        public async Task<Transaction> UpdateAsync(Transaction command)
        {
            var entity = _mapper.Map<Entities.Transaction>(command);
            entity.StatusId = command.Status!.Id;
            entity.CountryId = command.Country!.Id;
            entity.CustomerId = command.Customer!.Id;
            if (command.Gateway != null)
                entity.GatewayApplicationId = command.Gateway!.Id;

            await UpdateAsync(entity, entity.TransactionId);
            return command;
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await GetAsync(id);
            await DeleteAsync(entity);
        }

        #endregion
    }
}