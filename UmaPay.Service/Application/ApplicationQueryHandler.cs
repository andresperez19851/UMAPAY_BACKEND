namespace UmaPay.Service
{
    using Domain;
    using Interface.Repository;
    using Interface.Service;

    public class ApplicationQueryHandler : IApplicationQueryHandler
    {
        #region Properties

        private readonly IApplicationQueryRepository _applicationQueryRepository;

        #endregion Properties

        #region Constrcutor
        public ApplicationQueryHandler(IApplicationQueryRepository applicationQueryRepository)
        {
            _applicationQueryRepository = applicationQueryRepository ?? throw new ArgumentNullException(nameof(applicationQueryRepository));
        }
        #endregion Constrcutor

        #region Public Methods

        public async Task<IEnumerable<Application>> GetAllAsync()
        {
            return await _applicationQueryRepository.GetAllAsync();
        }

        public async Task<Application> GetByApiKeyAsync(string apiKey)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
                throw new ArgumentException("API key cannot be null or whitespace.", nameof(apiKey));

            return await _applicationQueryRepository.GetByApiKeyAsync(apiKey);
        }

        public async Task<Application> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("ID must be greater than zero.", nameof(id));

            return await _applicationQueryRepository.GetByIdAsync(id);
        }

        #endregion Public Methods

    }
}

