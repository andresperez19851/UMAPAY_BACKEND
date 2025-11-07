using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace UmaPay.Api.Controllers
{
    using Domain;
    using Interface.Service;
    using Filters;
    using UmaPay.Resource;

    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class InvoicePaymentController : ControllerBase
    {
        #region Properties

        private readonly IInvoicePaymentStatusService _invoicePaymentStatusService;
        private readonly ILogger<InvoicePaymentController> _logger;

        #endregion Properties

        #region Constructor

        public InvoicePaymentController(
            IInvoicePaymentStatusService invoicePaymentStatusService,
            ILogger<InvoicePaymentController> logger)
        {
            _invoicePaymentStatusService = invoicePaymentStatusService ?? throw new ArgumentNullException(nameof(invoicePaymentStatusService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #endregion Constructor

        #region Api Methods

        [HttpPost("check-status")]
        [ProducesResponseType(typeof(InvoicePaymentStatusListResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        [ValidateModel]
        public async Task<ActionResult<InvoicePaymentStatusListResponse>> CheckInvoicePaymentStatus([FromBody] InvoicePaymentStatusRequest request)
        {
            var startTime = DateTime.UtcNow;
            var success = false;

            try
            {
                _logger.LogInformation("Invoice payment status check started at {StartTime} for {InvoiceCount} invoices",
                    startTime, request.Invoices.Count);

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state for invoice payment status check");
                    return BadRequest(ModelState);
                }

                if (request.Invoices == null || !request.Invoices.Any())
                {
                    _logger.LogWarning("Empty invoice list provided for payment status check");
                    return BadRequest(new ErrorResponse { Message = "La lista de facturas no puede estar vacía" });
                }

                var result = await _invoicePaymentStatusService.CheckInvoicePaymentStatusAsync(request);

                success = true;
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during invoice payment status check");
                return StatusCode(500, new ErrorResponse { Message = Message.UnexpectedError });
            }
            finally
            {
                var endTime = DateTime.UtcNow;
                var duration = endTime - startTime;
                _logger.LogInformation("Invoice payment status check completed at {EndTime}. Duration: {Duration}. Success: {Success}",
                    endTime, duration, success);
            }
        }

        #endregion Api Methods
    }
}