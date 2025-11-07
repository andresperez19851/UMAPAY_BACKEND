using Microsoft.Extensions.DependencyInjection;

namespace UmaPay.Shared
{
    using Interface.Shared;

    public static class DependencyContainer
    {

        public static IServiceCollection AddShared(this IServiceCollection services)
        {
            services.AddScoped<IHash, Hash>();
            services.AddScoped<IRandomStringGenerator, RandomStringGenerator>();
            services.AddScoped<IMailService, MailService>();

            return services;
        }

    }
}