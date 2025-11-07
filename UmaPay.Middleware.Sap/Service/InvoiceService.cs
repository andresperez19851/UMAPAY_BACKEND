using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace UmaPay.Middleware.Sap
{
    using Domain;
    using Interface.Integration.Middleware;
    using Microsoft.Extensions.Logging;

    public class InvoiceService : IInvoiceService
    {
        #region Properties

        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<InvoiceService> _logger;

        #endregion Properties

        #region Constructor

        public InvoiceService(HttpClient httpClient, IConfiguration configuration, ILogger<InvoiceService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
        }

        #endregion Constructor

        #region Public Methods

        public async Task<OperationResult<SapResponse>> ProcessSapPaymentAsync(IEnumerable<Invoice> invoices, string currency, string gatewayCode)
        {
            try
            {
                var sapEndpoint = _configuration["SAPSettings:PaymentEndpoint"];
                var username = _configuration["SAPSettings:Username"];
                var password = _configuration["SAPSettings:Password"];

                var paymentRequest = GeneratePaymentRequest(invoices, gatewayCode, currency);

                var jsonContent = JsonSerializer.Serialize(paymentRequest, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = null
                });

                _logger.LogInformation($"Sending payment request to SAP: {jsonContent}");

                var content = new StringContent(paymentRequest, Encoding.UTF8, "application/json");

                using var requestMessage = new HttpRequestMessage(HttpMethod.Get, sapEndpoint);
                requestMessage.Content = content;
                var authenticationString = $"{username}:{password}";
                var base64EncodedAuthenticationString = Convert.ToBase64String(Encoding.ASCII.GetBytes(authenticationString));
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Basic", base64EncodedAuthenticationString);

                var response = await _httpClient.SendAsync(requestMessage);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();

                    _logger.LogInformation($"SAP response: {responseContent}");

                    var sapResponse = JsonSerializer.Deserialize<SapResponse>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        Converters = { new SapResponseJsonConverter() }
                    });

                    if (sapResponse != null)
                    {
                        if (sapResponse.IsSuccess)
                        {
                            sapResponse.ResponseContent = responseContent;
                            return OperationResult<SapResponse>.SuccessResult(sapResponse);
                        }
                        else
                        {
                            var errorMessage = sapResponse.ET_RETURN.GetFormattedMessage();
                            return OperationResult<SapResponse>.Failure(errorMessage);
                        }
                    }

                    return OperationResult<SapResponse>.Failure("Invalid SAP response");
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    this._logger.LogError($"SAP request failed with status code {response.StatusCode}: {errorMessage}");
                    return OperationResult<SapResponse>.Failure($"Error processing SAP payment: {errorMessage}");
                }
            }
            catch (Exception ex)
            {
                return OperationResult<SapResponse>.Failure($"Unexpected error processing SAP payment: {ex.Message}");
            }
        }

        #endregion Public Methods

        #region Private Methods

        private string GeneratePaymentRequest(IEnumerable<Invoice> invoices, string gatewayCode, string currency = "COP")
        {
            var request = new
            {
                I_PLATAFORMA = gatewayCode,
                I_MONEDA = currency,
                I_FACTURA = GenerateInvoiceItems(invoices)
            };

            return SerializeToJson(request);
        }

        private List<KeyValuePair<string, object>> GenerateInvoiceItems(IEnumerable<Invoice>? invoices)
        {
            var items = new List<KeyValuePair<string, object>>();

            if (invoices != null)
            {
                foreach (var invoice in invoices)
                {
                    items.Add(new KeyValuePair<string, object>("item", new
                    {
                        FACTURA = invoice.Number,
                        MONTO = invoice.Amount.ToString("0.##")
                    }));
                }
            }

            return items;
        }

        private string SerializeToJson(object obj)
        {
            return JsonSerializer.Serialize(obj, new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = null,
                Converters = { new DuplicateKeyJsonConverter() }
            });
        }

        #endregion Private Methods

    }

}