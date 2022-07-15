using ActivityTrackerApp.Dtos;

namespace ActivityTrackerApp.Services
{
    /// <summary>
    /// Session service.
    /// </summary>
    public interface ISessionService
    {
        /// <summary>
        /// Gets all active sessions associated with the activity.
        /// </summary>
        /// <param name="currUserId">The ID of the current user.</param>
        /// <param name="activityId">
        /// The ID of activity the session is tied to.
        /// If this is <c>null</c>, this gets all active sessions if the user is an admin. 
        /// </param>
        /// <returns>
        /// All active sessions associated with the activity.
        /// <c>null</c> if an activity with the ID doesn't exist.
        /// </returns>
        Task<IEnumerable<SessionGetDto>> GetAllSessionsByActivityIdAsync(
            Guid currUserId,
            Guid? activityId);

        /// <summary>
        /// Gets the session with the ID.
        /// </summary>
        /// <param name="currUserId">The ID of the current user.</param>
        /// <param name="sessionId">The ID of the session to get.</param>
        /// <returns>The session.</returns>
        Task<SessionGetDto> GetSessionAsync(
            Guid currUserId,
            Guid sessionId);

        /// <summary>
        /// Creates a new session.
        /// </summary>
        /// <param name="currUserId">The ID of the current user.</param>
        /// <param name="newSessionDto">The new session object.</param>
        /// <returns>The new session object.</returns>
        Task<SessionGetDto> CreateSessionAsync(
            Guid currUserId,
            SessionCreateDto newSessionDto);

        /// <summary>
        /// Updates the session with the ID.
        /// </summary>
        /// <param name="currUserId">The ID of the current user.</param>
        /// <param name="sessionId">The ID of the session to delete.</param>
        /// <param name="updatedSessionDto">The update session object.</param>
        /// <returns>The update object.</returns>
        Task<SessionGetDto> UpdateSessionAsync(
            Guid currUserId,
            Guid sessionId,
            SessionUpdateDto updatedSessionDto);

        /// <summary>
        /// Deletes the session with the ID.
        /// </summary>
        /// <param name="currUserId">The ID of the current user.</param>
        /// <param name="sessionId">The ID of the session to delete.</param>
        /// <returns>Whether the delete was successful.</returns>
        Task<bool> DeleteSessionAsync(
            Guid currUserId,
            Guid sessionId); 

    }
}
