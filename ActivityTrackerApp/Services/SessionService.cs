using ActivityTrackerApp.Entities;
using AutoMapper;

namespace ActivityTrackerApp.Services
{
    /// <summary>
    /// Session service.
    /// </summary>
    public class SessionService : ISessionService
    {
        private readonly IDataContext _dbContext;
        private readonly IMapper _mapper;

        public SessionService(IDataContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
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