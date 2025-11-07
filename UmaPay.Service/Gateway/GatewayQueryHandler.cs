namespace UmaPay.Service
{
    using Domain;
    using Interface.Repository;
    using Interface.Service;

    public class GatewayQueryHandler : IGatewayQueryHandler
    {
        #region Properties

        private readonly IGatewayQueryRepository _gatewayQueryRepository;

        #endregion Properties

        #region Constrcutor
        public GatewayQueryHandler(IGatewayQueryRepository gatewayQueryRepository)
        {
            _gatewayQueryRepository = gatewayQueryRepository ?? throw new ArgumentNullException(nameof(gatewayQueryRepository));
        }
        #endregion Constrcutor

        #region Public Methods

        public async Task<IEnumerable<Gateway>> GetAllAsync()
        {
            return await _gatewayQueryRepository.GetAllAsync();
        }

        public async Task<IEnumerable<Gateway>> GetByApplicationAsync(int application)
        {
            return await _gatewayQueryRepository.GetByApplicationAsync(application);
        }


        public async Task<Gateway> GetByApplicationAsync(string apikey, string code)
        {
            return await _gatewayQueryRepository.GetByApplicationAsync(apikey, code);
        }

        #endregion Public Methods

    }
}