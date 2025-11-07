namespace UmaPay.Service
{
    using Interface.Repository;
    using Interface.Service;
    using Interface.Shared;
    using Domain;
    using Resource;

    public class GatewayCommandHandler : IGatewayCommandHandler
    {
        #region Properties

        private readonly IApplicationQueryRepository _applicationQueryRepository;
        private readonly IGatewayQueryRepository _gatewayQueryRepository;
        private readonly ICountryQueryRepository _countryQueryRepository;
        private readonly IGatewayApplicationCommandRepository _gatewayApplicationCommandRepository;
        private readonly IGatewayCountryCommandRepository _gatewayCountryCommandRepository;


        #endregion Properties

        #region Constructor

        public GatewayCommandHandler(IApplicationQueryRepository applicationQueryRepository,
            IGatewayQueryRepository gatewayQueryRepository,
            ICountryQueryRepository countryQueryRepository,
            IGatewayApplicationCommandRepository gatewayApplicationCommandRepository,
            IGatewayCountryCommandRepository gatewayCountryCommandRepository)
        {
            _applicationQueryRepository = applicationQueryRepository ?? throw new ArgumentNullException(nameof(applicationQueryRepository));
            _gatewayQueryRepository = gatewayQueryRepository ?? throw new ArgumentNullException(nameof(gatewayQueryRepository));
            _countryQueryRepository = countryQueryRepository ?? throw new ArgumentNullException(nameof(countryQueryRepository));
            _gatewayApplicationCommandRepository = gatewayApplicationCommandRepository ?? throw new ArgumentNullException(nameof(gatewayApplicationCommandRepository));
            _gatewayCountryCommandRepository = gatewayCountryCommandRepository ?? throw new ArgumentNullException(nameof(gatewayCountryCommandRepository));
        }

        #endregion Constructor

        #region Public Methods

        public async Task<OperationResult<bool>> CreateAsync(string gatewayName, string applicationName, string countryName)
        {
            var queryGayeway = await _gatewayQueryRepository.GetByCodeAsync(gatewayName);
            if (queryGayeway == null)
            {
                return OperationResult<bool>.Failure(Message.GatewayInvalid);
            }

            var queryApplication = await _applicationQueryRepository.GetByNameAsync(applicationName);
            if (queryApplication == null)
            {
                return OperationResult<bool>.Failure(Message.ApplicationInvalid);
            }

            var queryCountry = await _countryQueryRepository.GetByNameAsync(countryName);
            if (queryCountry == null)
            {
                return OperationResult<bool>.Failure(Message.CountryInvalid);
            }

            var gateway = await _gatewayQueryRepository.ExistsGatewayApplicationCountryAsync(queryGayeway.Id, queryApplication.Id, queryCountry.Id);
            if (gateway)
            {
                return OperationResult<bool>.Failure(Message.GatewayExists);
            }

            var gatewayApplication = new GatewayApplication
            {
                GatewayId = queryGayeway.Id,
                ApplicationId = queryApplication.Id,
                Gateway = queryGayeway,
                Application = queryApplication
            };
            await _gatewayApplicationCommandRepository.AddAsync(gatewayApplication);

            var gatewayCountry = new GatewayCountry
            {
                GatewayId = queryGayeway.Id,
                CountryId = queryCountry.Id,
                Gateway = queryGayeway,
                Country = queryCountry
            };
            await _gatewayCountryCommandRepository.AddAsync(gatewayCountry);


            return OperationResult<bool>.SuccessResult(true);
        }

        #endregion Public Methods

    }
}