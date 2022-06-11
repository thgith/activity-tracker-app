using ActivityTrackerApp.Dtos;

namespace ActivityTrackerApp.Services
{
    /// <summary>
    /// User service.
    /// </summary>
    public interface IUserService
    {
        // Task<bool> IsCurrentUserAuthorized(Guid currUserId, Guid? userIdOfResource, bool allowIfSameUser = true);

        Task<bool> IsAdmin(Guid userId);

        Task<EntityWithToken<UserLoginDto>> AuthenticateUserAsync(UserLoginDto userLoginDto);

        Task<EntityWithToken<UserRegisterDto>> RegisterUserAsync(UserRegisterDto userPostDto);

        Task<IEnumerable<UserGetDto>> GetAllUsersAsync(Guid currUserId);

        Task<UserGetDto> GetUserAsync(Guid currUserId, Guid userId);

        Task<UserUpdateDto> UpdateUserAsync(Guid currUserId, Guid userId, UserUpdateDto user);

        Task<bool> DeleteUserAsync(Guid currUserId, Guid userId);

        Task<bool> IsEmailTaken(string email);
    }
}
