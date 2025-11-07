using Microsoft.Extensions.Configuration;

namespace UmaPay.Service
{
    using Interface.Repository;
    using Interface.Service;
    using Domain;
    using Resource;

    public class AuthenticateQueryHandler : IAuthenticateQueryHandler
    {
        #region Peroperties

        private readonly IConfiguration _configuration;
        private readonly IApplicationQueryRepository _applicationQueryRepository;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        #endregion Peroperties

        #region Constructor

        public AuthenticateQueryHandler(
            IApplicationQueryRepository applicationQueryRepository,
            IConfiguration configuration,
            IJwtTokenGenerator jwtTokenGenerator)
        {
            _applicationQueryRepository = applicationQueryRepository ?? throw new ArgumentNullException(nameof(applicationQueryRepository));
            _jwtTokenGenerator = jwtTokenGenerator ?? throw new ArgumentNullException(nameof(jwtTokenGenerator));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        #endregion Constructor

        #region Public Methods

        public async Task<OperationResult<Authorization>> AuthenticateApplicationAsync(string apiKey, string secret)
        {
            var application = await _applicationQueryRepository.GetByApiKeyAsync(apiKey);

            if (application == null)
                return OperationResult<Authorization>.Failure(Message.ApplicationNotFound);

            if (!application.IsActive)
                return OperationResult<Authorization>.Failure(Message.ApplicationInactive);

            if (secret != application.Secret)
                return OperationResult<Authorization>.Failure(Message.InvalidCredentials);

            var transaction = Guid.NewGuid();
            var token = _jwtTokenGenerator.GenerateToken(application, transaction);
            var autorization = new Authorization
            {
                Token = token,
                Url = _configuration["ApiSettings:UrlPayment"]!,
                Transaction = transaction
            };

            return OperationResult<Authorization>.SuccessResult(autorization);
        }

        #endregion Public Methods
    }
}