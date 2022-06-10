using ActivityTrackerApp.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace ActivityTrackerApp.Controllers
{
    /// <summary>
    /// Activity endpoints.
    /// </summary>
    public interface IActivityController
    {
        /// <summary>
        /// Retrieves activities.
        /// </summary>
        /// <returns>List of activities.</returns>
        [HttpGet]
        Task<ActionResult<IEnumerable<ActivityDto>>> GetAllAsync();

        /// <summary>
        /// Get the activity with the given ID.
        /// </summary>
        /// <param name="activityId">The ID of the activity to get.</param>
        /// <returns>Task of the activity.</returns>
        [HttpGet]
        Task<ActionResult<ActivityDto>> GetAsync(Guid activityId);

        /// <summary>
        /// Create the new activity.
        /// </summary>
        /// <param name="activityPostDto">The activity model for the create.</param>
        /// <returns>Task of the newly created activity.</returns>
        [HttpPost]
        Task<ActionResult> PostAsync(ActivityPostDto activityPostDto);

        /// <summary>
        /// Update the activity.
        /// </summary>
        /// <param name="activityPutDto">The activity model for the update.</param>
        /// <returns>Task of the updated activity.</returns>
        [HttpPut("{activityId}")]
        Task<IActionResult> PutAsync(Guid activityId, ActivityPutDto activityPutDto);

        /// <summary>
        /// Delete the activity with the given ID.
        /// <summary>
        /// <param name="activityId">The ID of the activity to delete.</param>
        [HttpDelete("{activityId}")]
        Task<IActionResult> DeleteAsync(Guid activityId);
    }
}
