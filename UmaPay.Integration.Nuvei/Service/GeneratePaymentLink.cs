using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

namespace UmaPay.Integration.Nuvei;

using Interface.Integration.Nuvei;
using Domain;

public class GeneratePaymentLink : IGeneratePaymentLink
{
    #region Fields

    private readonly string _apiUrl;
    private readonly HttpClient _httpClient;
    private readonly IAuthTokenService _authTokenService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<GeneratePaymentLink> _logger;

    #endregion

    #region Constructor

    public GeneratePaymentLink(
         IConfiguration configuration,
         HttpClient httpClient,
         IAuthTokenService authTokenService,
         ILogger<GeneratePaymentLink> logger)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration), string.Format(PaymentLinkMessages.ArgumentNullMessage, nameof(configuration)));
        _apiUrl = configuration["Nuvei:ApiUrl"] ?? throw new InvalidOperationException(string.Format(PaymentLinkMessages.ConfigurationMissing, "Nuvei:ApiUrl"));
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient), string.Format(PaymentLinkMessages.ArgumentNullMessage, nameof(httpClient)));
        _authTokenService = authTokenService ?? throw new ArgumentNullException(nameof(authTokenService), string.Format(PaymentLinkMessages.ArgumentNullMessage, nameof(authTokenService)));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger), string.Format(PaymentLinkMessages.ArgumentNullMessage, nameof(logger)));
    }

    #endregion

    #region Public Methods

    public async Task<OperationResult<Transaction>> GeneratePaymentLinkAsync(Transaction domain)
    {
        try
        {
            _logger.LogInformation(PaymentLinkMessages.GeneratingPaymentLink, domain.Id);

            var request = MapTransactionToRequest(domain);
            var jsonContent = SerializeRequest(request);
            domain.GatewayRequest = jsonContent;

            var response = await SendRequestAsync(jsonContent);
            var responseContent = await response.Content.ReadAsStringAsync();
            domain.GatewayResponse = responseContent;

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning(PaymentLinkMessages.FailedToGeneratePaymentLink, domain.Id, response.StatusCode);
                return OperationResult<Transaction>.Failure(PaymentLinkMessages.PaymentLinkError, domain);
            }

            var paymentLinkResponse = DeserializeResponse(responseContent);

            if (paymentLinkResponse.Success)
            {
                UpdateDomainWithResponse(domain, paymentLinkResponse);
                _logger.LogInformation(PaymentLinkMessages.SuccessfullyGeneratedPaymentLink, domain.Id);
                return OperationResult<Transaction>.SuccessResult(domain);
            }
            else
            {
                _logger.LogWarning(PaymentLinkMessages.PaymentLinkGenerationFailed, domain.Id, paymentLinkResponse.Detail);
                return OperationResult<Transaction>.Failure(string.Format(PaymentLinkMessages.PaymentLinkErrorStatus, paymentLinkResponse.Detail));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, PaymentLinkMessages.ErrorGeneratingPaymentLink, domain.Id);
            return OperationResult<Transaction>.Failure(ex.Message, domain);
        }
    }

    #endregion

    #region Private Methods

    private PaymentLinkRequest MapTransactionToRequest(Transaction transactionDomain)
    {
        return new PaymentLinkRequest
        {
            User = new PaymentLinkRequestCustomer
            {
                Id = transactionDomain.Id.ToString(),
                Email = transactionDomain.Customer?.Email ?? throw new ArgumentException(PaymentLinkMessages.CustomerEmailRequired),
                Name = transactionDomain.Customer?.FirstName ?? throw new ArgumentException(PaymentLinkMessages.CustomerFirstNameRequired),
                LastName = !string.IsNullOrWhiteSpace(transactionDomain.Customer?.LastName) ?
                    transactionDomain.Customer?.LastName : transactionDomain.Customer?.FirstName ?? throw new ArgumentException(PaymentLinkMessages.CustomerLastNameRequired),
            },
            Configuration = new PaymentLinkRequestConfiguration
            {
                PartialPayment = true,
                ExpirationDays = GetConfigurationValue<int>("Nuvei:ExpirationDays", 1),
                AllowedPaymentMethods = GetAllowedPaymentMethods(),
                SuccessUrl = transactionDomain.Url?.SuccessUrl ?? throw new ArgumentException(PaymentLinkMessages.SuccessUrlRequired),
                FailureUrl = transactionDomain.Url?.FailureUrl ?? throw new ArgumentException(PaymentLinkMessages.FailureUrlRequired),
                PendingUrl = transactionDomain.Url?.PendingUrl ?? throw new ArgumentException(PaymentLinkMessages.PendingUrlRequired),
                ReviewUrl = transactionDomain.Url?.ReviewUrl ?? throw new ArgumentException(PaymentLinkMessages.ReviewUrlRequired)
            },
            Order = new PaymentLinkRequestOrder
            {
                DevReference = transactionDomain.Id.ToString(),
                Description = transactionDomain.Description ?? throw new ArgumentException(PaymentLinkMessages.DescriptionRequired),
                Amount = transactionDomain.Amount,
                Currency = "COP",
                InstallmentsType = GetConfigurationValue<int>("Nuvei:InstallmentsType", 0)
            }
        };
    }

    private List<string> GetAllowedPaymentMethods()
    {
        var allowedMethods = _configuration.GetSection("Nuvei:AllowedPaymentMethods").Get<List<string>>();
        return allowedMethods ?? new List<string> { "All" };
    }

    private T GetConfigurationValue<T>(string key, T defaultValue)
    {
        return _configuration.GetValue<T>(key) ?? defaultValue;
    }

    private string SerializeRequest(PaymentLinkRequest request)
    {
        return JsonSerializer.Serialize(request, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        });
    }

    private async Task<HttpResponseMessage> SendRequestAsync(string jsonContent)
    {
        var token = _authTokenService.GenerateToken();
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("Auth-Token", token);
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        return await _httpClient.PostAsync($"{_apiUrl}/linktopay/init_order/", content);
    }

    private PaymentLinkResponse DeserializeResponse(string responseContent)
    {
        return JsonSerializer.Deserialize<PaymentLinkResponse>(responseContent)
            ?? throw new JsonException(PaymentLinkMessages.DeserializationFailed);
    }

    private void UpdateDomainWithResponse(Transaction domain, PaymentLinkResponse response)
    {
        domain.Url!.PaymentUrl = response.Data.Payment.PaymentUrl;
        domain.Reference = response.Data.Order.Id;
        domain.ExpirationDate = DateTime.Parse(response.Data.Configuration.ExpirationDate);
    }

    #endregion
}