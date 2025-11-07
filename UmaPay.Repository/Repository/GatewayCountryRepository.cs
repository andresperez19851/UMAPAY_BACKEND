using AutoMapper;


namespace UmaPay.Repository
{

    using Domain;
    using Interface.Repository;
    using Microsoft.Extensions.Configuration;

    public class GatewayCountryRepository : GenericRepository<Entities.GatewayCountry>, IGatewayCountryCommandRepository
    {
        #region Properties

        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly DataContext _context;
        

        #endregion Properties

        #region Constructor

        public GatewayCountryRepository(DataContext context, IMapper mapper, IConfiguration configuration ) : base(context)
        {
            _context = context;
            _configuration = configuration;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        }

        #endregion Constructor

        #region Command Methods

        public async Task<GatewayCountry> AddAsync(GatewayCountry gatewaycountry)
        {
            if (gatewaycountry == null)
                throw new ArgumentNullException(nameof(gatewaycountry));

            var entity = _mapper.Map<Entities.GatewayCountry>(gatewaycountry);
            var addedEntity = await AddAsyn(entity);
            return _mapper.Map<GatewayCountry>(addedEntity);
        }

        #endregion
    }

}