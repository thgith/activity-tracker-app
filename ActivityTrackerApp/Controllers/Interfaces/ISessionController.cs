using ActivityTrackerApp.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace ActivityTrackerApp.Controllers
{
    /// <summary>
    /// Session endpoints.
    /// </summary>
    public interface ISessiController
    {
        /// <summary>
        /// Retrieves sessions.
        /// </summary>
        /// <returns>List of sessions.</returns>
        [HttpGet]
        Task<ActionResult<IEnumerable<SessionGetDto>>> GetAllAsync();

        /// <summary>
        /// Get the Session with the given ID.
        /// </summary>
        /// <param name="sessionId">The ID of the session to get.</param>
        /// <returns>Task of the session.</returns>
        [HttpGet]
        Task<ActionResult<SessionGetDto>> GetAsync(Guid sessionId);

        /// <summary>
        /// Create the new session.
        /// </summary>
        /// <param name="SessionPostDto">The session model for the create.</param>
        /// <returns>Task of the newly created session.</returns>
        [HttpPost]
        Task<ActionResult> PostAsync(SessionCreateDto sessionPostDto);

        /// <summary>
        /// Update the session.
        /// </summary>
        /// <param name="SessionPutDto">The session model for the update.</param>
        /// <returns>Task of the updated session.</returns>
        [HttpPut("{sessionId}")]
        Task<IActionResult> PutAsync(Guid sessionId, SessionUpdateDto sessionPutDto);

        /// <summary>
        /// Delete the Session with the given ID.
        /// <summary>
        /// <param name="SessionId">The ID of the Session to delete.</param>
        [HttpDelete("{sessionId}")]
        Task<IActionResult> DeleteAsync(Guid sessionId);
    }
}
