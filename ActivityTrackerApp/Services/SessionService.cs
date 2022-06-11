using ActivityTrackerApp.Dtos;
using ActivityTrackerApp.Entities;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

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

        public async Task<IEnumerable<SessionGetDto>> GetAllSessionsAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        public async Task<SessionGetDto> GetSessionAsync(Guid sessionId)
        {
            throw new NotImplementedException();
        }
        
        public async Task<Session> CreateSessionAsync(SessionCreateDto createSessionDto)
        {
            throw new NotImplementedException();
        }

        public async Task<Session> UpdateSessionAsync(SessionUpdateDto updatedSessionDto)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> DeleteSessionAsync(Guid sessionId)
        {
            // Get active session with ID
            var session = await _getActiveSession(sessionId);

            // Return if session doesn't exist (or the session was already soft deleted)
            if (sessionId == null)
            {
                return false;
            }

            // Soft delete the session
            session.DateDeleted = DateTime.UtcNow;

            // Save to DB
            // SaveChangesAsync returns the number of entries written to the DB
            await _dbContext.SaveChangesAsync();
            return true;
        }

        private async Task<Session> _getActiveSession(Guid sessionId)
        {
            return await _dbContext.Sessions
                .FirstOrDefaultAsync(x => x.Id == sessionId && x.DateDeleted == null);
        }
    }
}