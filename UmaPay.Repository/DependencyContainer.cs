using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace UmaPay.Repository
{
    using Interface.Repository;

    public static class DependencyContainer
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            //services.AddScoped<DbContext, DataContext>();

            //services.AddDbContext<DataContext>(options => options.UseSqlServer(configuration.GetConnectionString("DataConnection")));

            // Registra DbContext como DataContext
            services.AddScoped<DbContext>(provider => provider.GetService<DataContext>());


            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            services.AddScoped<IApplicationCommandRepository, ApplicationRepository>();
            services.AddScoped<IApplicationQueryRepository, ApplicationRepository>();

            services.AddScoped<ICountryCommandRepository, CountryRepository>();
            services.AddScoped<ICountryQueryRepository, CountryRepository>();

            services.AddScoped<ICustomerQueryRepository, CustomerRepository>();
            services.AddScoped<ICustomerCommandRepository, CustomerRepository>();


            services.AddScoped<IGatewayCountryCommandRepository, GatewayCountryRepository>();


            services.AddScoped<IGatewayApplicationCommandRepository, GatewayApplicationRepository>();
            services.AddScoped<IGatewayApplicationQueryRepository, GatewayApplicationRepository>();

            services.AddScoped<IGatewayCommandRepository, GatewayRepository>();
            services.AddScoped<IGatewayQueryRepository, GatewayRepository>();

            services.AddScoped<ITransactionInvoiceQueryRepository, TransactionInvoiceRepository>();
            services.AddScoped<ITransactionInvoiceCommandRepository, TransactionInvoiceRepository>();


            services.AddScoped<ITransactionQueryRepository, TransactionRepository>();
            services.AddScoped<ITransactionCommandRepository, TransactionRepository>();

            services.AddScoped<ITransactionStatusLogCommandRepository, TransactionStatusLogRepository>();

            return services;
        }

    }
}