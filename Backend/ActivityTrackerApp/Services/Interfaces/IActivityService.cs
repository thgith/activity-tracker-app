using ActivityTrackerApp.Dtos;

namespace ActivityTrackerApp.Services
{
    /// <summary>
    /// Activity service.
    /// </summary>
    public interface IActivityService
    {
        /// <summary>
        /// Gets all active activities associated with the user.
        /// </summary>
        /// <param name="currUserId">The ID of the current user.</param>
        /// <param name="ownerId">
        /// The ID of user the activity is tied to.
        /// If this is <c>null</c>, this gets all active activities if the current user is an admin. 
        /// </param>
        /// <param name="includeSessions">
        /// Whether to include the sessions associated with the activity.
        /// </param>
        /// <returns>
        /// All active activities associated with the user.
        /// <c>null</c> if a user with the ID doesn't exist.
        /// </returns>
        Task<IEnumerable<ActivityGetDto>> GetAllActivitiesForUserAsync(
            Guid currUserId,
            Guid? ownerId,
            bool includeSessions = true);

        /// <summary>
        /// Gets the activity with the ID.
        /// </summary>
        /// <param name="currUserId">The ID of the current user.</param>
        /// <param name="activityId">The ID of the activity to get.</param>
        /// <param name="includeSessions">
        /// Whether to include the sessions associated with the activity.
        /// </param>
        /// <returns>The activity.</returns>
        Task<ActivityGetDto> GetActivityAsync(
            Guid currUserId,
            Guid activityId,
            bool includeSessions = true);

        /// <summary>
        /// Creates a new activity.
        /// </summary>
        /// <param name="currUserId">The ID of the current user.</param>
        /// <param name="ownerId">The ID of the user who owns the activity.</param>
        /// <param name="newActivityDto">The new activity object.</param>
        /// <returns>The new activity object.</returns>
        Task<ActivityGetDto> CreateActivityAsync(
            Guid currUserId,
            Guid ownerId,
            ActivityCreateDto newActivityDto);

        /// <summary>
        /// Updates the activity with the ID.
        /// </summary>
        /// <param name="currUserId">The ID of the current user.</param>
        /// <param name="activityId">The ID of the activity to delete.</param>
        /// <param name="updatedActivityDto">The update activity object.</param>
        /// <returns>The update object.</returns>
        Task<ActivityGetDto> UpdateActivityAsync(
            Guid currUserId,
            Guid activityId,
            ActivityUpdateDto updatedActivityDto);

        /// <summary>
        /// Deletes the activity with the ID.
        /// </summary>
        /// <param name="currUserId">The ID of the current user.</param>
        /// <param name="activityId">The ID of the activity to delete.</param>
        /// <returns>Whether the delete was successful.</returns>
        Task<bool> DeleteActivityAsync(
            Guid currUserId,
            Guid activityId);      
    }
}