using ActivityTrackerApp.Dtos;

namespace ActivityTrackerApp.Services
{
    /// <summary>
    /// User service.
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Checks whether the user is an admin.
        /// </summary>
        /// <param name="userId">The ID of the user to check.</param>
        /// <returns>Whether the user is an admin</returns>
        Task<bool> IsAdmin(Guid userId);

        /// <summary>
        /// Logs the user in.
        /// </summary>
        /// <param name="userLoginDto">The user credentials to log in.</param>
        /// <returns>An object of the user info with a JWT token.</param>
        Task<EntityWithToken<UserGetDto>> AuthenticateUserAsync(UserLoginDto userLoginDto);

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="newUserDto">The new user object.</param>
        /// <returns>An object of the new user with a JWT token.</param>
        Task<EntityWithToken<UserGetDto>> RegisterUserAsync(UserRegisterDto newUserDto);

        Task<bool> ChangePassword(Guid currUserId, string newPassword);

        /// <summary>
        /// Gets all active users.
        /// </summary>
        /// <param name="currUserId">The ID of the current user.</param>
        /// <returns>All the active users.</returns>
        Task<IEnumerable<UserGetDto>> GetAllUsersAsync(Guid currUserId);

        /// <summary>
        /// Gets the user with the ID.
        /// </summary>
        /// <param name="currUserId">The ID of the current user.</param>
        /// <param name="userId">The ID of the user to get.</param>
        /// <returns>The desired user.</returns>
        Task<UserGetDto> GetUserAsync(Guid currUserId, Guid userId);

        /// <summary>
        /// Updates the user with the ID.
        /// </summary>
        /// <param name="currUserId">The ID of the current user.</param>
        /// <param name="userId">The ID of the user to update.</param>
        /// <param name="updatedUserDto">The update object.</param>
        /// <returns>The update object.</returns>
        Task<UserGetDto> UpdateUserAsync(
            Guid currUserId,
            Guid userId,
            UserUpdateDto updatedUserDto);

        /// <summary>
        /// Deletes the user with the ID.
        /// </summary>
        /// <param name="currUserId">The ID of the current user.</param>
        /// <param name="userId">The ID of the user to delete.</param>
        /// <returns>Whether the delete was successful.</returns>
        Task<bool> DeleteUserAsync(Guid currUserId, Guid userId);

        /// <summary>
        /// Checks whether the email is already taken by another user.
        /// </summary>
        /// <param name="email">The email to check.</param>
        /// <returns>Whether the email is taken.</returns>
        Task<bool> IsEmailTaken(string email);
    }
}
