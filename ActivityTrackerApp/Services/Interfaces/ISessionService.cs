using ActivityTrackerApp.Entities;

namespace ActivityTrackerApp.Services
{
    /// <summary>
    /// Session service.
    /// </summary>
    public interface ISessionService
    {
        Task<Session> GetSessionAsync(Guid sessionId);
        
        Task<Session> CreateSessionAsync(Session newSession);
        
        Task<Session> UpdateSessionAsync(Session updatedSession);

        Task<bool> DeleteSessionAsync(Guid sessionId);
    }
}
