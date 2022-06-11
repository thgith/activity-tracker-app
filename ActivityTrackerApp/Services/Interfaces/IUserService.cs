using ActivityTrackerApp.Dtos;

namespace ActivityTrackerApp.Services
{
    /// <summary>
    /// User service.
    /// </summary>
    public interface IUserService
    {
        Task<bool> IsCurrentUserAuthorized(Guid currUserId, Guid? userIdOfResource, bool allowIfSameUser = true);

        Task<bool> IsAdmin(Guid userId);

        Task<EntityWithToken<UserUpdateDto>> AuthenticateUserAsync(UserUpdateDto userPutDto);

        Task<EntityWithToken<UserRegisterDto>> RegisterUserAsync(UserRegisterDto userPostDto);

        Task<IEnumerable<UserGetDto>> GetAllUsersAsync();

        Task<UserGetDto> GetUserAsync(Guid userId);

        Task<UserUpdateDto> UpdateUserAsync(Guid userId, UserUpdateDto user);

        Task<bool> DeleteUserAsync(Guid userId);

        Task<bool> IsEmailTaken(string email);
    }
}
