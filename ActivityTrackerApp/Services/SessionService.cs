using ActivityTrackerApp.Entities;

namespace ActivityTrackerApp.Services
{
    /// <summary>
    /// Session service.
    /// </summary>
    public class SessionService : ISessionService
    {
        private readonly IDataContext _dbContext;

        public SessionService(IDataContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public Task<Session> CreateSessionAsync(Session newSession)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteSessionAsync(Guid sessionId)
        {
            throw new NotImplementedException();
        }

        public Task<Session> GetSessionAsync(Guid sessionId)
        {
            throw new NotImplementedException();
        }

        public Task<Session> UpdateSessionAsync(Session updatedSession)
        {
            throw new NotImplementedException();
        }
    }
}