namespace UmaPay.Service
{
    using Interface.Repository;
    using Interface.Service;
    using Interface.Shared;
    using Domain;
    using Resource;

    public class ApplicationCommandHandler : IApplicationCommandHandler
    {
        #region Properties

        private readonly IApplicationCommandRepository _applicationCommandRepository;
        private readonly IApplicationQueryRepository _applicationQueryRepository;

        private readonly IRandomStringGenerator _ramdomStringGenerator;

        #endregion Properties

        #region Constructor

        public ApplicationCommandHandler(IApplicationCommandRepository applicationCommandRepository,
            IApplicationQueryRepository applicationQueryRepository,
            IRandomStringGenerator ramdomStringGenerator)
        {
            _applicationCommandRepository = applicationCommandRepository ?? throw new ArgumentNullException(nameof(applicationCommandRepository));
            _applicationQueryRepository = applicationQueryRepository ?? throw new ArgumentNullException(nameof(applicationQueryRepository));
            _ramdomStringGenerator = ramdomStringGenerator ?? throw new ArgumentNullException(nameof(ramdomStringGenerator));
        }

        #endregion Constructor

        #region Public Methods

        public async Task<OperationResult<Application>> CreateAsync(string applicationName)
        {
            // Validación del nombre de la aplicación
            if (string.IsNullOrWhiteSpace(applicationName))
            {
                return OperationResult<Application>.Failure(Message.ApplicationNameRequired);
            }

            if (applicationName.Length < 3 || applicationName.Length > 100)
            {
                return OperationResult<Application>.Failure(Message.ApplicationNameLenght);
            }

            if (await _applicationQueryRepository.ExistsByNameAsync(applicationName))
            {
                return OperationResult<Application>.Failure(Message.ApplicationNameExists);
            }
     

            var apiKey = await _ramdomStringGenerator.GenerateAsync(32);
            var secret = await _ramdomStringGenerator.GenerateAsync(64);

            var newApplication = new Application
            {
                Name = applicationName,
                ApiKey = apiKey,
                Secret = secret,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                LastUpdated = DateTime.UtcNow
            };

            newApplication = await _applicationCommandRepository.AddAsync(newApplication);

            return OperationResult<Application>.SuccessResult(newApplication);
        }

        public async Task<OperationResult<string>> RegenerateSecretAsync(string apiKey)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                return OperationResult<string>.Failure(Message.ApplicationApiKeyRequired);
            }

            var application = await _applicationQueryRepository.GetByApiKeyAsync(apiKey);
            if (application == null)
            {
                return OperationResult<string>.Failure(Message.ApplicationNotFound);
            }

            var newSecret = await _ramdomStringGenerator.GenerateAsync(64);
            application.Secret = newSecret;
            application.LastUpdated = DateTime.UtcNow;
            await _applicationCommandRepository.UpdateAsync(application);

            return OperationResult<string>.SuccessResult(newSecret);

        }


        #endregion Public Methods

    }
}