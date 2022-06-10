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
    public class SessionController : ApiControllerBase, ISessiController
    {
        private readonly ISessionService _sessionService;
        private readonly ILogger _logger;

        public SessionController(
            ISessionService sessionService,
            ILogger logger)
        {
            _sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [Authorize]
        public Task<IActionResult> DeleteAsync(Guid sessionId)
        {
            throw new NotImplementedException();
        }

        [Authorize]
        public Task<ActionResult<IEnumerable<SessionDto>>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        [Authorize]
        public Task<ActionResult<SessionDto>> GetAsync(Guid sessionId)
        {
            throw new NotImplementedException();
        }

        [Authorize]
        public Task<ActionResult> PostAsync(SessionPostDto sessionPostDto)
        {
            throw new NotImplementedException();
        }

        [Authorize]
        public Task<IActionResult> PutAsync(Guid sessionId, SessionPutDto sessionPutDto)
        {
            throw new NotImplementedException();
        }
    }
}
