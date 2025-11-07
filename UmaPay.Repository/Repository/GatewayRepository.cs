using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace UmaPay.Repository
{
    using Interface.Repository;
    using Microsoft.Extensions.Configuration;
    using UmaPay.Domain;

    public class GatewayRepository : GenericRepository<Entities.Gateway>, IGatewayQueryRepository, IGatewayCommandRepository
    {
        private readonly IMapper _mapper;
        private readonly DataContext _dataContext;

        public GatewayRepository(DataContext context, IMapper mapper) : base(context)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _dataContext = context ?? throw new ArgumentNullException(nameof(context));
        }

        #region Query Methods

        public async Task<Gateway> GetByIdAsync(int id)
        {
            var entity = await GetAsync(id);
            return _mapper.Map<Gateway>(entity);
        }

        public async Task<Gateway> GetByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be null or whitespace.", nameof(name));

            var entity = await FindAsync(g => g.Name == name);
            return _mapper.Map<Gateway>(entity);
        }

        public async Task<Gateway> GetByCodeAsync(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("Code cannot be null or whitespace.", nameof(code));

            var entity = await FindAsync(g => g.Code == code);
            return _mapper.Map<Gateway>(entity);
        }

        public async Task<IEnumerable<Gateway>> GetAllAsync()
        {
            var entities = await GetAllAsyn();
            return _mapper.Map<IEnumerable<Gateway>>(entities);
        }

        public async Task<IEnumerable<Gateway>> GetByApplicationAsync(int application)
        {
            var data = await (from gateway in _dataContext.Gateways
                              join gatewayApplication in _dataContext.GatewayApplications on gateway.GatewayId equals gatewayApplication.GatewayId
                              join gatewayCountry in _dataContext.GatewayCountries on gateway.GatewayId equals gatewayCountry.GatewayId
                              join countries in _dataContext.Countries on gatewayCountry.CountryId equals countries.CountryId
                              join applications in _dataContext.Applications on gatewayApplication.ApplicationId equals applications.ApplicationId
                              where applications.ApplicationId == application
                              select new Gateway
                              {
                                  Id = gatewayApplication.GatewayApplicationId,
                                  Code = gateway.Code,
                                  Name = gateway.Name,
                                  Currency = countries.CurrencyCode
                              })
                             .ToListAsync();
            return data;
        }

        public async Task<Gateway> GetByApplicationAsync(string apikey, string code)
        {
            var data = await (from gateway in _dataContext.Gateways
                              join gatewayApplication in _dataContext.GatewayApplications on gateway.GatewayId equals gatewayApplication.GatewayId
                              join applications in _dataContext.Applications on gatewayApplication.ApplicationId equals applications.ApplicationId
                              where gateway.Code == code
                              && applications.ApiKey == apikey
                              select new Gateway
                              {
                                  Id = gatewayApplication.GatewayApplicationId,
                                  Code = gateway.Code,
                                  Name = gateway.Name
                              })
                             .FirstOrDefaultAsync();
            return data;
        }

        public async Task<Gateway> GetByApplicationAsync(int application, string code, string currency)
        {
            var data = await (from gateway in _dataContext.Gateways
                              join gatewayApplication in _dataContext.GatewayApplications on gateway.GatewayId equals gatewayApplication.GatewayId
                              join gatewayCountry in _dataContext.GatewayCountries on gateway.GatewayId equals gatewayCountry.GatewayId
                              join countries in _dataContext.Countries on gatewayCountry.CountryId equals countries.CountryId
                              join applications in _dataContext.Applications on gatewayApplication.ApplicationId equals applications.ApplicationId
                              where gateway.Code == code
                              && applications.ApplicationId == application
                              && countries.CurrencyCode == currency
                              select new Gateway
                              {
                                  Id = gatewayApplication.GatewayApplicationId,
                                  Code = gateway.Code,
                                  Name = gateway.Name,
                                  Currency = countries.CurrencyName
                              })
                             .FirstOrDefaultAsync();
            return data;
        }

        public async Task<bool> ExistsGatewayApplicationCountryAsync(int gatewayId, int applicationId, int countryId)
        {
            var exists = await _dataContext.Gateways
                .AnyAsync(gateway =>
                    gateway.GatewayId == gatewayId &&
                    gateway.GatewayApplications.Any(ga => ga.ApplicationId == applicationId) &&
                    gateway.GatewayCountries.Any(gc => gc.CountryId == countryId));

            return exists;
        }

        #endregion

        #region Command Methods

        public async Task<Gateway> AddAsync(Gateway gateway)
        {
            if (gateway == null)
                throw new ArgumentNullException(nameof(gateway));

            var entity = _mapper.Map<Entities.Gateway>(gateway);
            var addedEntity = await AddAsyn(entity);
            return _mapper.Map<Gateway>(addedEntity);
        }

        public async Task<Gateway> UpdateAsync(Gateway gateway)
        {
            if (gateway == null)
                throw new ArgumentNullException(nameof(gateway));

            var entity = _mapper.Map<Entities.Gateway>(gateway);
            var updatedEntity = await UpdateAsync(entity, entity.GatewayId);
            return _mapper.Map<Gateway>(updatedEntity);
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await GetAsync(id);
            if (entity == null)
                throw new ArgumentException($"Gateway with id {id} not found.");

            await DeleteAsync(entity);
        }

        #endregion
    }

}