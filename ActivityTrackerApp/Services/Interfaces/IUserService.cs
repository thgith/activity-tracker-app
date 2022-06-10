using ActivityTrackerApp.Dtos;
using ActivityTrackerApp.Entities;

namespace ActivityTrackerApp.Services
{
    /// <summary>
    /// User service.
    /// </summary>
    public interface IUserService
    {
        Task<User> AuthenticateAsync(UserPutDto userPutDto);
        Task<UserPostDtoWithToken> RegisterUserAsync(UserPostDto userPostDto);

        string GenerateJwtToken(User user);

        Task<IEnumerable<UserDto>> GetAllUsersAsync();

        Task<UserDto> GetUserAsync(Guid userId);

        Task<UserPostDto> CreateUserAsync(UserPostDto user);

        Task<UserPutDto> UpdateUserAsync(Guid userId, UserPutDto user);

        Task<bool> DeleteUserAsync(Guid userId);

        Task<bool> IsEmailTaken(string email);
    }
}
