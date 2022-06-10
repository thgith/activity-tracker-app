using ActivityTrackerApp.Dtos;
using ActivityTrackerApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace ActivityTrackerApp.Controllers
{
    /// <summary>
    /// Session endpoints.
    /// </summary>
    [Route("api/v1/Session")]
    public class SessionController : ApiControllerBase, ISessiController
    {
        private readonly ISessionService _sessionService;

        public SessionController(ISessionService sessionService)
        {
            _sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));
        }

        public Task<IActionResult> DeleteAsync(Guid sessionId)
        {
            throw new NotImplementedException();
        }

        public Task<ActionResult<IEnumerable<SessionDto>>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ActionResult<SessionDto>> GetAsync(Guid sessionId)
        {
            throw new NotImplementedException();
        }

        public Task<ActionResult> PostAsync(SessionPostDto sessionPostDto)
        {
            throw new NotImplementedException();
        }

        public Task<IActionResult> PutAsync(Guid sessionId, SessionPutDto sessionPutDto)
        {
            throw new NotImplementedException();
        }
    }
}
