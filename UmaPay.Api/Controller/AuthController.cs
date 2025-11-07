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
    public class AuthController : ControllerBase
    {
        #region Properties

        private readonly IAuthenticateQueryHandler _authenticateQueryHandler;
        private readonly ITransactionCommandHandler _transactionCommandHandler;
        private readonly IGatewayQueryHandler _gatewayQueryHandle;

        private readonly ILogger<AuthController> _logger;

        #endregion Properties

        #region Constructor

        public AuthController(
             IAuthenticateQueryHandler authenticateQueryHandler,
             ITransactionCommandHandler transactionCommandHandler,
             IGatewayQueryHandler gatewayQueryHandle,
             ILogger<AuthController> logger)
        {
            _authenticateQueryHandler = authenticateQueryHandler ?? throw new ArgumentNullException(nameof(authenticateQueryHandler));
            _transactionCommandHandler = transactionCommandHandler ?? throw new ArgumentNullException(nameof(transactionCommandHandler));
            _gatewayQueryHandle = gatewayQueryHandle ?? throw new ArgumentNullException(nameof(gatewayQueryHandle));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #endregion Constructor

        #region Api Methods

        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        [ValidateModel]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
        {
            var startTime = DateTime.UtcNow;
            var success = false;
            //request.Gateway = "DAVI_COL";
            try
            {
                _logger.LogInformation(string.Format(Message.AuthController_LoginAttemptStarted, startTime, request.ApiKey));

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning(string.Format(Message.AuthController_InvalidModelState, request.ApiKey));
                    return BadRequest(ModelState);
                }

                var result = await _authenticateQueryHandler.AuthenticateApplicationAsync(request.ApiKey, request.Secret);
                if (!result.Success)
                {
                    _logger.LogWarning(string.Format(Message.AuthController_AuthenticationFailed, request.ApiKey, result.Message));
                    return BadRequest(new ErrorResponse { Message = result.Message });
                }

                Gateway gateway = new Gateway();
                if (!string.IsNullOrEmpty(request.Gateway))
                {
                    gateway = await _gatewayQueryHandle.GetByApplicationAsync(request.ApiKey, request.Gateway);
                }
                else
                    gateway = null;


                var transaction = MapRequestToTransaction(request.Customer, request.Invoices, gateway, request.Country, result.Data.Transaction);
                var resultTransaction = await _transactionCommandHandler.CreateAsync(transaction);

                if (!resultTransaction.Success)
                {
                    _logger.LogWarning(string.Format(Message.AuthController_TransactionCreationFailed, request.ApiKey, resultTransaction.Message));
                    return BadRequest(new ErrorResponse { Message = resultTransaction.Message });
                }

                success = true;
                return Ok(new LoginResponse { Token = result.Data.Token, Url = result.Data.Url, Transaction = resultTransaction.Data.Token });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Format(Message.AuthController_UnexpectedErrorMessage, request.ApiKey));
                return StatusCode(500, new ErrorResponse { Message = Message.UnexpectedError });
            }
            finally
            {
                var endTime = DateTime.UtcNow;
                var duration = endTime - startTime;
                _logger.LogInformation(string.Format(Message.AuthController_LoginAttemptCompleted, endTime, request.ApiKey, duration, success));
            }
        }

        #endregion Api Methods

        #region Map

        private Transaction MapRequestToTransaction(CustomerPaymentRequest customer, List<InvoicePaymentRequest> invoice, Gateway? gateway, string country, Guid token)
        {
            return new Transaction
            {
                Amount = invoice.Sum(i => i.Amount),
                TransactionDate = DateTime.UtcNow,
                Token = token,
                Customer = new Customer
                {
                    FirstName = customer.FirstName,
                    LastName = customer.LastName,
                    Email = customer.Email,
                    CodeSap = customer.CodeSap,
                    Society = customer.Society,
                    User = customer.User
                },
                Invoice = invoice.Select(i => new Invoice
                {
                    Number = i.Number,
                    Amount = i.Amount,
                    Total = i.Total,
                    IsPaid = i.Total.HasValue && i.Total.Value == i.Amount,
                    TotalToPay = i.Total - i.Amount
                }).ToList(),
                Country = new Country
                {
                    Id = 0,
                    Name = country
                },
                Gateway = gateway
            };
        }

        #endregion Map

    }
}