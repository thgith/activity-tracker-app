using ActivityTrackerApp.Dtos;
using ActivityTrackerApp.Entities;

namespace ActivityTrackerApp.Services
{
    /// <summary>
    /// Session service.
    /// </summary>
    public interface ISessionService
    {        
        Task<IEnumerable<SessionGetDto>> GetAllSessionsByActivityIdAsync(
            Guid currUserId,
            Guid? activityId);

        Task<SessionGetDto> GetSessionAsync(
            Guid currUserId,
            Guid sessionId);

        Task<SessionCreateDto> CreateSessionAsync(
            Guid currUserId,
            SessionCreateDto newSessionDto);

        Task<SessionUpdateDto> UpdateSessionAsync(
            Guid currUserId,
            Guid sessionId,
            SessionUpdateDto updateSessionDto);

        Task<bool> DeleteSessionAsync(
            Guid currUserid,
            Guid sessionId); 

    }
}
