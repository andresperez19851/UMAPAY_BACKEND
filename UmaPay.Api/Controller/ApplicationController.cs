using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace UmaPay.Api.Controller
{
    using Azure.Core;
    using Domain;
    using Interface.Service;    
    using UmaPay.Api.Attributes;

    [ApiController]
    [Route("api/[controller]")]
    [ApiKeyAuth]
    public class ApplicationController : ControllerBase
    {

        #region Properties

        private readonly IApplicationCommandHandler _applicationCommand;
        private readonly IApplicationQueryHandler _applicationQuery;

        #endregion  Properties

        #region Constructor

        public ApplicationController(IApplicationCommandHandler applicationCommand, IApplicationQueryHandler applicationQuery)
        {
            _applicationCommand = applicationCommand ?? throw new ArgumentNullException(nameof(applicationCommand));
            _applicationQuery = applicationQuery ?? throw new ArgumentNullException(nameof(applicationQuery));
        }

        #endregion Constructor

        #region Api Methods

        [HttpPost]
        [ProducesResponseType(typeof(ApplicationResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateApplication([FromBody] CreateApplicationRequest request)
        {
            if (!ModelState.IsValid)
            {
                throw new ValidationException(ModelState.ToString());
            }

            var result = await _applicationCommand.CreateAsync(request.Name);
            if (!result.Success)
            {
                throw new ValidationException(result.Message);
            }

            var response = MapToApplicationResponse(result.Data);
            return CreatedAtAction(nameof(GetApplication), new { id = response.Id }, response);
        }

        [HttpPost("regenerate-secret")]
        [ProducesResponseType(typeof(ApplicationSecretResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RegenerateSecret([FromBody] SecretApplicationRequest request)
        {
            if (!ModelState.IsValid)
            {
                throw new ValidationException(ModelState.ToString());
            }

            var result = await _applicationCommand.RegenerateSecretAsync(request.ApiKey);

            if (!result.Success)
            {
                throw new ValidationException(result.Message);
            }

            var response = new ApplicationSecretResponse
            {
                NewSecret = result.Data
            };
            return Ok(response);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApplicationResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetApplication(int id)
        {
            var application = await _applicationQuery.GetByIdAsync(id);
            if (application == null)
            {
                return NotFound();
            }
            var response = MapToApplicationResponse(application);
            return Ok(response);
        }

        #endregion Api Methods

        #region Mapping

        private static ApplicationResponse MapToApplicationResponse(Application application)
        {
            return new ApplicationResponse
            {
                Id = application.Id,
                Name = application.Name,
                ApiKey = application.ApiKey,
                Secret = application.Secret,
                IsActive = application.IsActive,
                CreatedAt = application.CreatedAt,
                LastUpdated = application.LastUpdated,
            };
        }

        #endregion Mapping

    }
}