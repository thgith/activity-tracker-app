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
            IUserService userService,
            IJwtService jwtService,
            ISessionService sessionService,
            ILogger<SessionController> logger) : base(userService, jwtService, logger)
        {
            _sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));
        }

        public Task<ActionResult<IEnumerable<SessionDto>>> GetAllSessionsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ActionResult<SessionDto>> GetSessionAsync(Guid sessionId)
        {
            throw new NotImplementedException();
        }

        public Task<ActionResult> CreateSessionAsync(SessionPostDto sessionPostDto)
        {
            throw new NotImplementedException();
        }

        public Task<IActionResult> UpdateSessionAsync(Guid sessionId, SessionPutDto sessionPutDto)
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
