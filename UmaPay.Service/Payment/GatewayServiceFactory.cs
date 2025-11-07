using Microsoft.Extensions.DependencyInjection;

namespace UmaPay.Service
{
    using Interface.Service;
    using UmaPay.Resource;
    using UmaPay.Service.Wompi;

    public class GatewayServiceFactory : IGatewayServiceFactory
    {

        #region Properties

        private readonly IServiceProvider _serviceProvider;

        #endregion Properties

        #region Constructor
        public GatewayServiceFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        #endregion Constructor

        #region Public Methods
        public IGatewayService GetGatewayService(string gatewayCode)
        {
            return gatewayCode.ToUpper() switch
            {
                ConstGateway.GatewayNuvei => _serviceProvider.GetRequiredService<NuveiGatewayServiceAdapter>(),
                ConstGateway.GatewayWompy => _serviceProvider.GetRequiredService<WompiGatewayServiceAdapter>(),
                _ => throw new ArgumentException($"Unsupported gateway: {gatewayCode}")
            };
        }

        #endregion Public Methods
    }

}