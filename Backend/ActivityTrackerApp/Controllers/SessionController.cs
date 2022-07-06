using ActivityTrackerApp.Dtos;
using ActivityTrackerApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace ActivityTrackerApp.Controllers;

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
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllSessionsForActivityAsync(
        [FromQuery] Guid activityId)
    {
        async Task<IActionResult> GetAllSessionsForActivityPartialAsync(Guid currUserGuid)
        {
            var sessions = await _sessionService.GetAllSessionsByActivityIdAsync(currUserGuid, activityId);
            return Ok(sessions);
        }
        return await checkAuthAndPerformAction(Request, GetAllSessionsForActivityPartialAsync);
    }

    [HttpGet("{sessionId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetSessionAsync(Guid sessionId)
    {
        async Task<IActionResult> GetSessionPartialAsync(Guid currUserGuid)
        {
            var session = await _sessionService.GetSessionAsync(currUserGuid, currUserGuid);
            return Ok(session);
        }
        return await checkAuthAndPerformAction(Request, GetSessionPartialAsync);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateSessionAsync([FromBody] SessionCreateDto sessionCreateDto)
    {
        async Task<IActionResult> CreateSessionPartialAsync(Guid currUserGuid)
        {
            var session = await _sessionService.CreateSessionAsync(currUserGuid, sessionCreateDto);
            return Ok(session);
        }
        return await checkAuthAndPerformAction(Request, CreateSessionPartialAsync);
    }

    [HttpPut("{sessionId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateSessionAsync(
        Guid sessionId,
        [FromBody] SessionUpdateDto sessionUpdateDto)
    {
        async Task<IActionResult> UpdateSessionPartialAsync(Guid currUserGuid)
        {
            var session = await _sessionService.UpdateSessionAsync(currUserGuid, sessionId, sessionUpdateDto);
            if (session == null)
            {
                return Problem($"The session with the ID {sessionId} does not exist", statusCode: StatusCodes.Status404NotFound);
            }

            return Ok(session);
        }

        return await checkAuthAndPerformAction(Request, UpdateSessionPartialAsync);
    }

    [HttpDelete("{sessionId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteSessionAsync(Guid sessionId)
    {
        async Task<IActionResult> DeleteSessionPartialAsync(Guid currUserGuid)
        {
            var isSuccess = await _sessionService.DeleteSessionAsync(currUserGuid, sessionId);
            if (!isSuccess)
            {
                return Problem($"Session with ID {sessionId} does not exist", statusCode: StatusCodes.Status404NotFound);
            }
            return Ok($"Successfully deleted session with ID {sessionId}");
        }
        return await checkAuthAndPerformAction(Request, DeleteSessionPartialAsync);
    }
}