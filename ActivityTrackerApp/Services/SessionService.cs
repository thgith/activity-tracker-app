using ActivityTrackerApp.Dtos;
using ActivityTrackerApp.Entities;
using ActivityTrackerApp.Exceptions;
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
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public SessionService(
            IDataContext dbContext,
            IUserService userService,
            IMapper mapper)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        // <returns>
        // <c>null<c>if the activity doesn't exist
        // </returns>
        public async Task<IEnumerable<SessionGetDto>> GetAllSessionsByActivityIdAsync(
            Guid currUserId,
            Guid activityId)
        {
            // Check if the activity exists first
            var activity = await _dbContext.Activities.FirstOrDefaultAsync(x => 
                                x.Id == activityId && 
                                x.DeletedDateUtc == null);

            if (activity == null)
            {
                return null;
            }

            // Regular users can get sessions associated with their own activities.
            // Admins can get everyone's session.
            if (currUserId != activity.OwnerId && !(await _userService.IsAdmin(currUserId)))
            {
                throw new ForbiddenException();
            }

            // Get all active sessions belonging to the user
            var sessions = await _dbContext.Sessions
                .Where(x => x.ActivityId == activityId && x.DateDeletedUtc == null)
                .OrderBy(x => x.StartDateUtc)
                .ToListAsync();       
            
            // Convert entity to DTO and return
            return sessions.Select(x => _mapper.Map<SessionGetDto>(x)); 
        }

        public async Task<SessionGetDto> GetSessionAsync(Guid currUserId, Guid sessionId)
        {
            var session = await _getActiveSession(sessionId);

            // Session doesn't exist
            if (session == null)
            {
                return null;
            }

            var activity = await _dbContext.Activities.FirstOrDefaultAsync(x =>
                            x.Id == session.ActivityId &&
                            x.DateDeletedUtc == null);

            // Activity owning session doesn't exist for some reason
            if (activity == null)
            {
                return null;
            }

            // Regular users can get sessions associated with their own activities.
            // Admins can get everyone's session.
            if (currUserId != activity.OwnerId && !(await _userService.IsAdmin(currUserId)))
            {
                throw new ForbiddenException();
            }

            // Convert entity to DTO and return
            return _mapper.Map<SessionGetDto>(session);
        }

        public async Task<SessionCreateDto> CreateSessionAsync(Guid currUserId, Guid ownerId, SessionCreateDto newSessionDto)
        {
            throw new NotImplementedException();
            // Regular users can create their own activities. Admins can create activities for anyone.
            // if (currUserId != ownerId && !(await _userService.IsAdmin(currUserId)))
            // {
            //     throw new ForbiddenException();
            // }

            // // Convert DTO to entity
            // var activity =  _mapper.Map<Activity>(newActivityDto);

            // // Set the owner of the activity
            // activity.OwnerId = ownerId;

            // // Add activity
            // await _dbContext.Activities.AddAsync(activity);

            // // Save to DB
            // await _dbContext.SaveChangesAsync();

            // return newActivityDto;
        }

        public Task<SessionUpdateDto> UpdateSessionAsync(Guid currUserId, Guid sessionId, SessionUpdateDto updateSessionDto)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> DeleteSessionAsync(Guid currUserid, Guid sessionId)
        {
            // Get active session with ID
            var session = await _getActiveSession(sessionId);

            // Return if session doesn't exist (or the session was already soft deleted)
            if (session == null)
            {
                return false;
            }

            // Can only delete a session belonging to the user or if admin
            if (currUserid != session.Activity.OwnerId && !(await _userService.IsAdmin(currUserid)))
            {
                throw new ForbiddenException();
            }

            // Soft delete the session
            session.DateDeletedUtc = DateTime.UtcNow;

            // Save to DB
            // SaveChangesAsync returns the number of entries written to the DB
            await _dbContext.SaveChangesAsync();
            return true;        
        }

        private async Task<Session> _getActiveSession(Guid sessionId)
        {
            return await _dbContext.Sessions
                .FirstOrDefaultAsync(x => x.Id == sessionId && x.DateDeletedUtc == null);
        }
    }
}