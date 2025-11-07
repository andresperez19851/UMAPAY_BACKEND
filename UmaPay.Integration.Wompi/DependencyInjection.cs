using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using UmaPay.Integration.Wompi.Constants;
using UmaPay.Integration.Wompi.Mappings;
using UmaPay.Integration.Wompi.Service;
using UmaPay.Integration.Wompi.Settings;
using UmaPay.Interface.Integration.Nuvei;
using UmaPay.Interface.Service;
using UmaPay.Resource;

namespace UmaPay.Integration.Wompi;

public static class DependencyInjection
{
    public static IServiceCollection AddWompiIntegration(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<WompiSettings>(options =>
        {
            configuration.GetSection(nameof(WompiSettings)).Bind(options);

            var societiesSection = configuration.GetSection($"{nameof(WompiSettings)}:Societies");
            if (societiesSection.Exists())
            {
                var societiesList = societiesSection.Get<List<SocietySettings>>();
                if (societiesList != null)
                {
                    options.Societies = societiesList.ToDictionary(
                        s => s.Society,
                        s => s
                    );
                }
            }
        });

        services.AddKeyedScoped<IGeneratePaymentLink, GeneratePaymentLink>(ConstGateway.GatewayWompy);
        services.AddHttpClient(WompiIntegrationConstants.WompiHttpClient, (sp, client) =>
        {
            var settings = sp.GetRequiredService<IOptions<WompiSettings>>().Value;
            client.BaseAddress = new Uri(settings.BaseAddress!);
            client.Timeout = TimeSpan.FromSeconds(settings.TimeoutInSeconds!.Value);
        });

        services.AddScoped<IWompiVerifySignatureService, WompiVerifySignatureService>();

        services.AddKeyedScoped<IStatusMapper, StatusMapper>(ConstGateway.GatewayWompy);

        return services;
    }
}