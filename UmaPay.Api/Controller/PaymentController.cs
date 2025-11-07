using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace UmaPay.Api.Controllers
{
    using Interface.Service;
    using UmaPay.Domain;
    using Resource;

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        #region Properties

        private readonly ITransactionCommandHandler _paymentCommand;
        private readonly ILogger<PaymentController> _logger;

        #endregion Properties

        #region Constructor

        public PaymentController(ITransactionCommandHandler paymentCommand, ILogger<PaymentController> logger)
        {
            _paymentCommand = paymentCommand ?? throw new ArgumentNullException(nameof(paymentCommand));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #endregion Constructor

        #region Api Methods

        [HttpPost("generate-link")]
        [ProducesResponseType(typeof(PaymentResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PaymentResponse>> GeneratePaymentLink([FromBody] GatewayPaymentRequest paymentRequest)
        {
            var startTime = DateTime.UtcNow;
            var success = false;

            try
            {
                _logger.LogInformation(string.Format(Message.PaymentController_GenerateLinkAttemptStarted, startTime, paymentRequest.Code));

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning(string.Format(Message.PaymentController_InvalidModelState, paymentRequest.Code));
                    return BadRequest(ModelState);
                }

                var application = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var transaction = User.FindFirst(ConstClaim.Transaction)?.Value;

                if (string.IsNullOrEmpty(application) || string.IsNullOrEmpty(transaction))
                {
                    _logger.LogWarning(string.Format(Message.PaymentController_UnauthorizedAccess, application, transaction));
                    return Unauthorized(Message.JWTGatewayRequired);
                }

                var request = MapPaymentRequestToTransaction(paymentRequest, application);

                var result = await _paymentCommand.GenereLinkAsync(Guid.Parse(transaction), request, paymentRequest.email);

                if (!result.Success)
                {
                    _logger.LogWarning(string.Format(Message.PaymentController_LinkGenerationFailed, paymentRequest.Code, result.Message));
                    return BadRequest(result.Message);
                }

                var response = MapTransactionToPaymentResponse(result.Data);
                success = true;
                _logger.LogInformation(string.Format(Message.PaymentController_LinkGenerationSucceeded, paymentRequest.Code, response.TransactionId));
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, Message.PaymentController_UnexpectedError, paymentRequest.Code);
                return StatusCode(500, new ErrorResponse { Message = Message.UnexpectedError });
            }
            finally
            {
                var endTime = DateTime.UtcNow;
                var duration = endTime - startTime;
                _logger.LogInformation(string.Format(Message.PaymentController_GenerateLinkAttemptCompleted, endTime, paymentRequest.Code, duration, success));
            }
        }

        #endregion Api Methods

        #region Map

        private Gateway MapPaymentRequestToTransaction(GatewayPaymentRequest request, string application)
        {
            return new Gateway
            {
                Id = int.Parse(application),
                Code = request.Code
            };
        }

        private PaymentResponse MapTransactionToPaymentResponse(Transaction transaction)
        {
            return new PaymentResponse
            {
                TransactionId = transaction.Id,
                Amount = transaction.Amount,
                Status = transaction.Status.Name,
                PaymentLink = transaction.PaymentUrl
            };
        }

        #endregion Map

    }
}