using ActivityTrackerApp.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace ActivityTrackerApp.Controllers
{
    /// <summary>
    /// User endpoints.
    /// </summary>
    public interface IUserController
    {
        /// <summary>
        /// Retrieves users.
        /// </summary>
        /// <returns>List of users</returns>
        [HttpGet]
        Task<ActionResult<IEnumerable<UserDto>>> GetAllAsync();

        /// <summary>
        /// Get the user with the given ID.
        /// </summary>
        /// <param name="userId">The ID of the user to get</param>
        /// <returns>Task of the user.</returns>
        [HttpGet]
        Task<ActionResult<UserDto>> GetAsync(Guid userId);

        /// <summary>
        /// Create the new user.
        /// </summary>
        /// <param name="userDto">The user model for the create.</param>
        /// <returns>Task of the newly created user.</returns>
        [HttpPost]
        Task<ActionResult> PostAsync(UserPostDto userPostDto);

        /// <summary>
        /// Update the user.
        /// </summary>
        /// <param name="userDto">The user model for the update.</param>
        /// <returns>Task of the updated user.</returns>
        [HttpPut("{userId}")]
        Task<IActionResult> PutAsync(Guid userId, UserPutDto userPutDto);

        /// <summary>
        /// Delete the user with the given ID.
        /// <summary>
        /// <param name="userId">The GUID of the user to delete.</param>
        [HttpDelete("{userId}")]
        Task<IActionResult> DeleteAsync(Guid userId);
    }
}