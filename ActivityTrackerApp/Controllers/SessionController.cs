using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ActivityTrackerApp.Constants;
using ActivityTrackerApp.Dtos;
using ActivityTrackerApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ActivityTrackerApp.Controllers
{
    /// <summary>
    /// Session endpoints.
    /// </summary>
    [Route("api/v1/[controller]")]
    public class SessionController : ApiControllerBase<SessionController>
    {
        private readonly ISessionService _sessionService;

        public SessionController(
            ISessionService sessionService,
            IUserService userService,
            IJwtService jwtService,
            ILogger<SessionController> logger) : base(userService, jwtService, logger)
        {
            _sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));
        }

        /// <summary>
        /// Get all sessions associated with the user and activity
        /// </summary>
        public async Task<ActionResult<IEnumerable<SessionGetDto>>> GetAllSessionsAsync(
            Guid activityId)
        {
            throw new NotImplementedException();
        }

        public Task<ActionResult<SessionGetDto>> GetSessionAsync(
            Guid sessionId)
        {
            throw new NotImplementedException();
        }

        public Task<ActionResult> CreateSessionAsync(SessionCreateDto sessionPostDto)
        {
            throw new NotImplementedException();
        }

        public Task<IActionResult> UpdateSessionAsync(Guid sessionId, SessionUpdateDto sessionPutDto)
        {
            throw new NotImplementedException();
        }

        [HttpDelete("{sessionId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public Task<IActionResult> DeleteSessionAsync(Guid sessionId)
        {
            throw new NotImplementedException();
        }
    }
}
