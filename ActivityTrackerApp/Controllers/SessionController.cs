using ActivityTrackerApp.Services;

namespace ActivityTrackerApp.Controllers
{
    /// <summary>
    /// Session endpoints.
    /// </summary>
    public class SessionController : ApiControllerBase
    {
        private readonly ISessionService _sessionService;

        public SessionController(ISessionService sessionService)
        {
            _sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));
        }

    }
}
