using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace UmaPay.Api.Controller
{
    using Domain;
    using Interface.Service;
    using Resource;
    using UmaPay.Api.Attributes;
    using UmaPay.Api.Helpers;

    [ApiController]
    [Route("api/[controller]")]

    public class GatewayController : ControllerBase
    {
        #region Properties

        private readonly IGatewayQueryHandler _gatewayQuery;
        private readonly IGatewayCommandHandler _gatewayCommand;
        private readonly IUserContextService _userContextService;
        private readonly ILogger<GatewayController> _logger;

        #endregion Properties

        #region Constructor

        public GatewayController(
             IGatewayQueryHandler gatewayQuery,
             IGatewayCommandHandler gatewayCommand,
             IUserContextService userContextService,
             ILogger<GatewayController> logger)
        {
            _gatewayQuery = gatewayQuery ?? throw new ArgumentNullException(nameof(gatewayQuery));
            _gatewayCommand = gatewayCommand ?? throw new ArgumentNullException(nameof(gatewayCommand));
            _userContextService = userContextService ?? throw new ArgumentNullException(nameof(userContextService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #endregion Constructor

        #region Api Methods

        [Authorize]
        [HttpGet]
        [ProducesResponseType(typeof(ApplicationResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get()
        {
            var startTime = DateTime.UtcNow;
            var success = false;

            try
            {
                _logger.LogInformation(string.Format(Message.GatewayController_GetAttemptStarted, startTime));

                var applicationCode = _userContextService.GetApplicationCode(User);
                if (applicationCode == null)
                {
                    _logger.LogWarning(string.Format(Message.GatewayController_UnauthorizedAccess));
                    return Unauthorized(Message.JWTGatewayRequired);
                }

                _logger.LogInformation(string.Format(Message.GatewayController_ApplicationCodeRetrieved, applicationCode.Value));

                var response = await _gatewayQuery.GetByApplicationAsync(applicationCode.Value);
                if (response == null)
                {
                    _logger.LogWarning(string.Format(Message.GatewayController_GatewaysNotFound, applicationCode.Value));
                    return NotFound();
                }

                success = true;
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Format(Message.GatewayController_UnexpectedError));
                return StatusCode(StatusCodes.Status500InternalServerError, Message.UnexpectedError);
            }
            finally
            {
                var endTime = DateTime.UtcNow;
                var duration = endTime - startTime;
                _logger.LogInformation(string.Format(Message.GatewayController_GetAttemptCompleted, endTime, duration, success));
            }
        }

        [ApiKeyAuth]
        [HttpGet("all")]
        [ProducesResponseType(typeof(IEnumerable<Gateway>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllGateways()
        {
            var gateways = await _gatewayQuery.GetAllAsync();
            if (gateways == null || !gateways.Any())
            {
                return NotFound(Message.GatewayNotFound);
            }

            return Ok(gateways);

        }


        [ApiKeyAuth]
        [HttpPost]
        [ProducesResponseType(typeof(Gateway), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateGateway([FromBody] CreateGatewayRequest request)
        {
            if (!ModelState.IsValid)
            {
                throw new ValidationException(ModelState.ToString());
            }

            var result = await _gatewayCommand.CreateAsync(request.Gateway, request.Application, request.Country);
            if (!result.Success)
            {
                throw new ValidationException(result.Message);
            }

            return Ok(new { message = Message.Controller_CreateSucess });


        }
        
        #endregion Api Methods
    }
}
