using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;
using UmaPay.Domain;
using UmaPay.Integration.Wompi.Constants;
using UmaPay.Integration.Wompi.Contracts.PaymentLink.Request;
using UmaPay.Integration.Wompi.Contracts.PaymentLink.Response;
using UmaPay.Integration.Wompi.Mappings;
using UmaPay.Integration.Wompi.Resource;
using UmaPay.Integration.Wompi.Settings;
using UmaPay.Interface.Integration.Nuvei;
using UmaPay.Resource;

namespace UmaPay.Integration.Wompi.Service;

public sealed class GeneratePaymentLink(
    IHttpClientFactory httpClientFactory,
    IOptions<WompiSettings> wompiSettings,
    ILogger<GeneratePaymentLink> logger) : IGeneratePaymentLink
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient(WompiIntegrationConstants.WompiHttpClient);
    private readonly WompiSettings _wompiSettings = wompiSettings.Value;
    private readonly ILogger<GeneratePaymentLink> _logger = logger;

    /// <summary>
    /// Genera el enlace de pago asincrónicamente para una transacción.
    /// </summary>
    /// <param name="transaction">La transacción para la cual se generará el enlace de pago.</param>
    /// <returns>El resultado de la operación que contiene la transacción con el enlace de pago generado.</returns>
    public async Task<OperationResult<Transaction>> GeneratePaymentLinkAsync(Transaction transaction)
    {
        _logger.LogInformation(PaymentLinkMessages.GeneratingPaymentLink(transaction.Id));

        var redirectUrl = BuildRedirectUrl(transaction.Token.ToString());

        var paymentLinkRequest = transaction.ToWompiRequest(
            _wompiSettings.TransactionName!,
            _wompiSettings.ExpirationInDays!,
            redirectUrl);

        var serializedRequest = SerializeRequest(paymentLinkRequest);

        transaction.GatewayRequest = serializedRequest;

        if (!_wompiSettings!.Societies!.TryGetValue(transaction.Customer!.Society, out var societySettings))
        {
            _logger.LogWarning(PaymentLinkMessages.SocietyNotFound(transaction.Customer.Society));
            SetTransactionStatusToFailed(transaction);
            return OperationResult<Transaction>.Failure(PaymentLinkMessages.SocietyNotFound(transaction.Customer.Society), transaction);
        }

        var response = await CreatePaymentLinkAsync(serializedRequest, societySettings.PrivateKey);
        var responseContent = await response.Content.ReadAsStringAsync();

        transaction.GatewayResponse = responseContent;

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning(PaymentLinkMessages.ErrorCreatingPaymentLink(transaction.Id, (int)response.StatusCode));
            SetTransactionStatusToFailed(transaction);
            return OperationResult<Transaction>.Failure(PaymentLinkMessages.ErrorAtCreatingPaymentLink, transaction);
        }
        else
        {
            _logger.LogInformation(PaymentLinkMessages.SuccessfullyGeneratedPaymentLink(transaction.Id));
            UpdateTransactionWithGatewayResponse(transaction, responseContent);
            return transaction;
        }
    }

    /// <summary>
    /// Crea el enlace de pago de forma asíncrona.
    /// </summary>
    /// <param name="request">La solicitud de enlace de pago.</param>
    /// <param name="privateKey">La llave privada de Wompi.</param>
    /// <returns>La respuesta HTTP del enlace de pago.</returns>
    private async Task<HttpResponseMessage> CreatePaymentLinkAsync(string request, string privateKey)
    {
        var content = new StringContent(request, Encoding.UTF8, "application/json");
        var requestMessage = new HttpRequestMessage(HttpMethod.Post, "payment_links")
        {
            Content = content,
        };

        requestMessage.Headers.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", privateKey);

        return await _httpClient.SendAsync(requestMessage);
    }

    /// <summary>
    /// Establece el estado de la transacción como fallido.
    /// </summary>
    /// <param name="transaction">La transacción.</param>
    private void SetTransactionStatusToFailed(Transaction transaction)
    {
        transaction.Status = new TransactionStatus
        {
            Description = ConstStatus.GetStatusDescription(ConstStatus.Failed),
            Id = ConstStatus.Failed,
            Name = ConstStatus.GetStatusName(ConstStatus.Failed)
        };
    }

    /// <summary>
    /// Actualiza la transacción con la respuesta del enlace de pago.
    /// </summary>
    /// <param name="transaction">La transacción.</param>
    /// <param name="responseContent">El contenido de la respuesta del enlace de pago.</param>
    private void UpdateTransactionWithGatewayResponse(Transaction transaction, string responseContent)
    {
        var paymentLinkRespone = JsonSerializer.Deserialize<PaymentLinkResponse>(responseContent);
        transaction.PaymentUrl = BuildPaymentUrl(paymentLinkRespone!.Data.Id);
        transaction.Reference = paymentLinkRespone.Data.Id;
        transaction.ExpirationDate = paymentLinkRespone.Data.ExpiresAt;
    }

    /// <summary>
    /// Construye la URL de pago.
    /// </summary>
    /// <param name="wompiId">El ID de Wompi.</param>
    /// <returns>La URL de pago.</returns>
    private string BuildPaymentUrl(string wompiId)
        => BuildUrl(_wompiSettings.CheckOutUrl!, wompiId);

    /// <summary>
    /// Construye la URL de redirección.
    /// </summary>
    /// <param name="token">El token.</param>
    /// <returns>La URL de redirección.</returns>
    private string BuildRedirectUrl(string token)
        => BuildUrl(_wompiSettings.RedirectUrl!, token);

    /// <summary>
    /// Construye una URL.
    /// </summary>
    /// <param name="baseAddress">La dirección base.</param>
    /// <param name="id">El ID.</param>
    /// <returns>La URL construida.</returns>
    private string BuildUrl(string baseAddress, string id) => $"{baseAddress}{id}";

    /// <summary>
    /// Serializa la solicitud de enlace de pago.
    /// </summary>
    /// <param name="request">La solicitud de enlace de pago.</param>
    /// <returns>La solicitud serializada.</returns>
    private string SerializeRequest(PaymentLinkRequest request)
    {
        return JsonSerializer.Serialize(
            request,
            new JsonSerializerOptions
            {
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            });
    }
}
