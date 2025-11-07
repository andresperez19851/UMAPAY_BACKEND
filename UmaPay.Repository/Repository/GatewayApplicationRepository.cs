using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace UmaPay.Repository
{

    using Domain;
    using Interface.Repository;
    using Microsoft.Extensions.Configuration;

    public class GatewayApplicationRepository : GenericRepository<Entities.GatewayApplication>, IGatewayApplicationQueryRepository, IGatewayApplicationCommandRepository
    {
        #region Properties

        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly DataContext _context;
        

        #endregion Properties

        #region Constructor

        public GatewayApplicationRepository(DataContext context, IMapper mapper, IConfiguration configuration ) : base(context)
        {
            _context = context;
            _configuration = configuration;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        }

        #endregion Constructor

        #region Query Methods

        public async Task<GatewayApplication> GetAsync(int gatewayId, int applicationId)
        {
            var entity = await FindAsync(ga => ga.GatewayId == gatewayId && ga.ApplicationId == applicationId);
            return _mapper.Map<GatewayApplication>(entity);
        }

        public async Task<IEnumerable<GatewayApplication>> GetByGatewayIdAsync(int gatewayId)
        {
            var entities = await FindAllAsync(ga => ga.GatewayId == gatewayId);
            return _mapper.Map<IEnumerable<GatewayApplication>>(entities);
        }

        public async Task<IEnumerable<GatewayApplication>> GetByApplicationIdAsync(int applicationId)
        {
            var entities = await FindAllAsync(ga => ga.ApplicationId == applicationId);
            return _mapper.Map<IEnumerable<GatewayApplication>>(entities);
        }

        public async Task<IEnumerable<GatewayApplication>> GetAllAsync()
        {
            var entities = await GetAllAsyn();
            return _mapper.Map<IEnumerable<GatewayApplication>>(entities);
        }

        public async Task<bool> IsGatewayAssociatedWithApplicationAsync(string apiKey, string gateway)
        {
            var data = await (from applications in _context.Applications
                              join gatewayApplications in _context.GatewayApplications on applications.ApplicationId equals gatewayApplications.ApplicationId
                              join gateways in _context.Gateways on gatewayApplications.GatewayId equals gateways.GatewayId
                              where applications.ApiKey == apiKey
                              && gateways.Code == gateway
                              select applications).FirstOrDefaultAsync();
            return data != null;
        }

        #endregion

        #region Command Methods

        public async Task<GatewayApplication> AddAsync(GatewayApplication gatewayApplication)
        {
            if (gatewayApplication == null)
                throw new ArgumentNullException(nameof(gatewayApplication));

            var entity = _mapper.Map<Entities.GatewayApplication>(gatewayApplication);
            var addedEntity = await AddAsyn(entity);
            return _mapper.Map<GatewayApplication>(addedEntity);
        }

        public async Task<GatewayApplication> UpdateAsync(GatewayApplication gatewayApplication)
        {
            if (gatewayApplication == null)
                throw new ArgumentNullException(nameof(gatewayApplication));

            var entity = _mapper.Map<Entities.GatewayApplication>(gatewayApplication);
            var updatedEntity = await UpdateAsync(entity, new object[] { entity.GatewayId, entity.ApplicationId });
            return _mapper.Map<GatewayApplication>(updatedEntity);
        }

        public async Task DeleteAsync(int gatewayId, int applicationId)
        {
            var entity = await FindAsync(ga => ga.GatewayId == gatewayId && ga.ApplicationId == applicationId);
            if (entity != null)
            {
                await DeleteAsync(entity);
            }
        }

        #endregion
    }

}