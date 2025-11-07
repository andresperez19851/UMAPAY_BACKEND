using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace UmaPay.IoC
{
    using Repository;
    using Service;
    using Shared;
    using Integration.Nuvei;
    using UmaPay.Middleware.Sap;
    using UmaPay.Integration.Wompi;

    public static class DependencyContainer
    {
        public static IServiceCollection AddNetDependency(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddRepositories(configuration);
            services.AddService(configuration);
            services.AddShared();
            services.AddIntegrationNuvei(configuration);
            services.AddWompiIntegration(configuration);
            services.AddMiddlewareSap(configuration);
            return services;
        }

    }
}