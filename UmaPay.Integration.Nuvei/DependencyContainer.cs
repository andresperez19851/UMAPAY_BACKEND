using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace UmaPay.Integration.Nuvei
{
    using Interface.Integration.Nuvei;
    using UmaPay.Resource;

    public static class DependencyContainer
    {
        public static IServiceCollection AddIntegrationNuvei(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IAuthTokenService, AuthTokenService>();
            services.AddKeyedScoped<IGeneratePaymentLink, GeneratePaymentLink>(ConstGateway.GatewayNuvei);
            services.AddKeyedScoped<IStatusMapper, StatusMapper>(ConstGateway.GatewayNuvei);
            return services;
        }
    }
}