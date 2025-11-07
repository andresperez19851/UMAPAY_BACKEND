using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace UmaPay.Api.Controllers;

using Interface.Service;
using Domain;
using Resource;
using Microsoft.Extensions.Logging;

[ApiController]
[Route("api/[controller]")]
public class NuveiController(
    INuveiCommandHandler nuveiCommand,
    IGatewayServiceFactory gatewayServiceFactory,
    ILogger<NuveiController> logger) : ControllerBase
{

    #region Properties

    private readonly INuveiCommandHandler _nuveiCommand = nuveiCommand;
    private readonly ILogger<NuveiController> _logger = logger;
    private readonly IGatewayServiceFactory _gatewayServiceFactory = gatewayServiceFactory;

    #endregion Properties

    #region Api Methods

    [HttpPost("receive")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status203NonAuthoritative)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ReceiveWebhook([FromBody] JsonElement requestBody)
    {
        try
        {

            var rawJson = requestBody.GetRawText();
            var nuveiRequest = JsonSerializer.Deserialize<NuveiRequest>(rawJson);

            if (nuveiRequest == null)
            {

                _logger.LogWarning(string.Format(Message.PaymentController_InvalidModelState, requestBody));
                return BadRequest(ModelState);
            }

            var verificationResult = await _nuveiCommand.Verify(
                nuveiRequest.Transaction.Stoken,
                nuveiRequest.Transaction.Id.ToString(),
                nuveiRequest.User.Id.ToString());

            if (!verificationResult.Success)
            {
                return StatusCode(203, new { error = Message.InvalidStoken });
            }

            var gatewayService = _gatewayServiceFactory.GetGatewayService(ConstGateway.GatewayNuvei);
            
            var processResult = await _nuveiCommand.ProcessAsync(
                nuveiRequest.Transaction.DevReference,
                gatewayService, 
                rawJson, 
                nuveiRequest.Transaction.Id);

            if (!processResult.Success)
            {
                return StatusCode(500, new { error = processResult.Message });
            }

            return Ok(new { message = Message.NuveiController_Sucess });
        }
        catch (JsonException jerror)
        {
            return BadRequest(Message.NuveiController_InvalidJson);
        }
        catch (Exception)
        {
            return StatusCode(500, new { error = Message.UnknownError });
        }
    }


    #endregion Api Methods

}