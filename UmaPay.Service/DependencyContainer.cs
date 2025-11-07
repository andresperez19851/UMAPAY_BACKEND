using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace UmaPay.Service
{
    using Interface.Service;
    using UmaPay.Service.Wompi;

    public static class DependencyContainer
    {
        public static IServiceCollection AddService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IApplicationCommandHandler, ApplicationCommandHandler>();
            services.AddScoped<IApplicationQueryHandler, ApplicationQueryHandler>();
            services.AddScoped<IAuthenticateQueryHandler, AuthenticateQueryHandler>();
            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

            services.AddScoped<ITransactionCommandHandler, TransactionCommandHandler>();
            services.AddScoped<IGatewayServiceFactory, GatewayServiceFactory>();

            services.AddScoped<IGatewayCommandHandler, GatewayCommandHandler>();
            services.AddScoped<IGatewayQueryHandler, GatewayQueryHandler>();

            services.AddScoped<ITransactionQueryHandler, TransactionQueryHandler>();
            services.AddScoped<ITransactionStatusCommandHandler, TransactionStatusCommandHandler>();

            // Registrar el adaptadores
            services.AddScoped<NuveiGatewayServiceAdapter>();
            services.AddScoped<WompiGatewayServiceAdapter>();
            services.AddScoped<INuveiCommandHandler, NuveiCommandHandler>();

            services.AddScoped<IWompiService, WompiService>();
            services.AddScoped<IInvoicePaymentStatusService, Payment.InvoicePaymentStatusService>();

            return services;
        }

    }
}