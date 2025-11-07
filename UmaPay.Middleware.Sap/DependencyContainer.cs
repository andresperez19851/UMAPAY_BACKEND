using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace UmaPay.Middleware.Sap
{
    using Interface.Integration.Middleware;
    using UmaPay.Middleware.Sap.Service;

    public static class DependencyContainer
    {
        public static IServiceCollection AddMiddlewareSap(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IInvoiceService, InvoiceService>();
            services.Configure<SAPSettings>(options => configuration.GetSection(nameof(SAPSettings)).Bind(options));

            return services;
        }

    }
}