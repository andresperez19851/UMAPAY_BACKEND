using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace UmaPay.Api.Controller
{
    using Domain;
    using Interface.Service;
    using Microsoft.Extensions.Options;
    using UmaPay.Api.Attributes;
    using UmaPay.Middleware.Sap.Service;
    using UmaPay.Resource;

    [ApiController]
    [Route("api/[controller]")]
    public class TransactionController(
        ITransactionQueryHandler transactionQuery,
        ITransactionStatusCommandHandler transactionStatusCommand,
        ILogger<TransactionController> logger,
        IOptions<SAPSettings> sapSettingsOptions) : ControllerBase
    {
        private readonly ITransactionQueryHandler _transactionQuery = transactionQuery ?? throw new ArgumentNullException(nameof(transactionQuery));
        private readonly ITransactionStatusCommandHandler _transactionStatusCommand = transactionStatusCommand ?? throw new ArgumentNullException(nameof(transactionStatusCommand));
        private readonly ILogger<TransactionController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly SAPSettings _sapSettings = sapSettingsOptions.Value;

        #region Api Methods

        [Authorize]
        [HttpGet(ApiRoutes.Transaction.Get)]
        [ProducesResponseType(typeof(TransactionResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<TransactionResponse>> Get()
        {
            var id = User.FindFirst(ConstClaim.Transaction)?.Value;
            if (string.IsNullOrEmpty(id))
            {
                _logger.LogWarning(Message.JWTGatewayRequired);
                return Unauthorized(Message.JWTGatewayRequired);
            }

            var result = await _transactionQuery.GetByTokenSingleAsync(Guid.Parse(id));
            if (!result.Success)
            {
                _logger.LogWarning(result.Message);
                return BadRequest(result.Message);
            }

            var response = MapToTransactionResponse(result.Data);
            return Ok(response);
        }

        [ApiKeyAuth]
        [HttpGet(ApiRoutes.Transaction.ByToken)]
        [ProducesResponseType(typeof(TransactionResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<TransactionResponse>> GetTransactionByToken(
            [FromRoute] Guid token, 
            [FromQuery] bool? logTransactionStatus = false)
        {
            var result = await _transactionQuery.GetByTokenSingleAsync(token, logTransactionStatus);
            if (!result.Success)
            {
                return NotFound(result.Message);
            }

            var response = MapToTransactionResponse(result.Data);
            return Ok(response);
        }

        [ApiKeyAuth]
        [HttpGet(ApiRoutes.Transaction.ByTokenFlag)]
        [ProducesResponseType(typeof(TransactionResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<TransactionResponse>> GetTransactionByToken([FromRoute] string flag, [FromRoute] Guid token)
        {
            var result = await _transactionQuery.GetByTokenAsync(token, flag);
            if (!result.Success)
            {
                return NotFound(result.Message);
            }

            var response = MapToTransactionResponse(result.Data);
            return Ok(response);
        }


        [ApiKeyAuth]
        [HttpGet(ApiRoutes.Transaction.Search)]
        [ProducesResponseType(typeof(IEnumerable<TransactionReport>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<TransactionReport>>> SearchTransactions([FromQuery] TransactionSearchModel searchModel)
        {

            if (!string.IsNullOrEmpty(searchModel.Customer) &&
                _sapSettings.ProcessFailedInSap.HasValue && 
                _sapSettings.ProcessFailedInSap.Value)
                await _transactionStatusCommand.ProcessFailedInSap(searchModel.Customer);

            var result = await _transactionQuery.SearchTransactionsAsync(
                searchModel.StartDate,
                searchModel.EndDate,
                searchModel.Status, 
                searchModel.Token, 
                searchModel.Customer);

            if (!result.Success)
            {
                return NotFound(result.Message);
            }

            return Ok(result.Data);
        }

        [ApiKeyAuth]
        [HttpGet(ApiRoutes.Transaction.ProcessFailedInSap)]
        [ProducesResponseType(typeof(TransactionResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<TransactionResponse>> ProcessFailedInSap([FromRoute] Guid token)
        {
            var result = await _transactionStatusCommand.ProcessFailedInSap(token);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return Ok(new { message = Message.Controller_UpdateSucess });
        }

        #endregion Api Methods

        #region Map

        private static TransactionResponse MapToTransactionResponse(Transaction transaction)
        {
            return new()
            {
                Id = transaction.Id,
                Amount = transaction.Amount,
                Token = transaction.Token,
                Reference = transaction.Reference,
                ExpirationDate = transaction.ExpirationDate,
                Status = new TransactionStatusResponse
                {
                    Id = transaction.Status!.Id,
                    Name = transaction.Status.Name,
                    Description = transaction.Status.Description
                },
                Customer = new CustomerResponse
                {
                    Id = transaction.Customer!.Id,
                    FirstName = transaction.Customer.FirstName,
                    LastName = transaction.Customer.LastName,
                    Email = transaction.Customer.Email,
                    CodeSap = transaction.Customer.CodeSap,
                    Society = transaction.Customer.Society
                },
                TransactionDate = transaction.TransactionDate,

                Invoices = transaction.Invoice?.Select(i => new InvoiceResponse
                {
                    Number = i.Number,
                    Amount = i.Amount,
                    Status = (i.Status == null) ? null : new TransactionStatusResponse { Id = i.Status.Id, Name = i.Status.Name, Description = i.Status.Description }
                }).ToList()!,

                Gateway = (transaction!.Gateway == null) ? null : new GatewayResponse
                {
                    Id = transaction.Gateway.Id,
                    Code = transaction.Gateway.Code,
                    Name = transaction.Gateway.Name
                },

                Country = new Country
                {
                    Name = transaction.Country!.Name,
                    CurrencyCode = transaction.Country.CurrencyCode,
                    CurrencyName = transaction.Country.CurrencyName
                },

            };
        }

        #endregion Map

        public static class ApiRoutes
        {
            public static class Transaction
            {
                public const string Get = "get";
                public const string ByToken = "by-token/{token}";
                public const string ByTokenFlag = "flag/{flag}/by-token/{token}";
                public const string Search = "search";
                public const string ProcessFailedInSap = "forward-sap/{token}";

            }
        }


    }
}