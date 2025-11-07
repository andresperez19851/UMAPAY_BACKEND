using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using UmaPay.Domain.Service.Wompi.Webhook;
using UmaPay.Interface.Service;
using UmaPay.Resource;

namespace UmaPay.Api.Controller;

[Route("api/wompi")]
[ApiController]
public class WompiController(
    ILogger<WompiController> logger,
    IWompiService wompiService) : ControllerBase
{
    private readonly ILogger<WompiController> _logger = logger;
    private readonly IWompiService _wompiService = wompiService;

    [HttpPost("receive-event")]
    public async Task<IActionResult> ReceiveWebhook([FromBody] JsonElement requestBody)
    {
        try
        {
            var rawRequestBody = requestBody.GetRawText();
            var webhookEventRequest = JsonSerializer.Deserialize<WompiEventRequest>(rawRequestBody);

            if (webhookEventRequest is null)
            {
                _logger.LogWarning(string.Format(Message.UnprocessableWompiEvent, requestBody));
                return BadRequest(ModelState);
            }

            var signatureVerificationResult = await _wompiService.VerifySignature(webhookEventRequest!);

            if (!signatureVerificationResult.Success)
            {
                return StatusCode(500, new { error = Message.UnknownError });
            }

            if (!signatureVerificationResult.Data)
            {
                return StatusCode(203, new { error = Message.InvalidStoken });
            }

            var processTransactionResult = await _wompiService.ProcessAsync(
                rawRequestBody,
                webhookEventRequest);

            if (!processTransactionResult.Success)
            {
                return StatusCode(500, new
                {
                    error = $"{Message.ErrorAtProcessWompiEvent} {webhookEventRequest.Data.Transaction.Reference}"
                });
            }

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, Message.UnexpectedError);
            return StatusCode(500, new { error = Message.UnknownError });
        }
    }
}