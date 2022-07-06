using ActivityTrackerApp.Database;
using ActivityTrackerApp.Dtos;
using ActivityTrackerApp.Entities;
using ActivityTrackerApp.Exceptions;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace ActivityTrackerApp.Services;

/// <inheritdoc/>
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

    /// <inheritdoc/>
    public async Task<IEnumerable<SessionGetDto>> GetAllSessionsByActivityIdAsync(
        Guid currUserId,
        Guid? activityId = null)
    {
        var isAdmin = await _userService.IsAdmin(currUserId);

        // No activityId means get all sessions. Only admins can do this
        if (activityId == null && !isAdmin)
        {
            throw new ForbiddenException();
        }

        // Else there is an associated activity to filter by
        // Check if the activity exists first
        var activity = await _dbContext.Activities.FirstOrDefaultAsync(x => 
                            x.Id == activityId && 
                            x.DeletedDateUtc == null);

        // Activity doesn't exist, so return
        if (activity == null)
        {
            return null;
        }

        // Regular users can get sessions associated with their own activities.
        // Admins can get everyone's session.
        if (currUserId != activity.OwnerId && !isAdmin)
        {
            throw new ForbiddenException();
        }

        // Get all active sessions
        var sessionsQuery = _dbContext.Sessions
            .Where(x => x.DeletedDateUtc == null);
        
        // Add filter on activity if exists
        if (activityId != null)
        {
            sessionsQuery = sessionsQuery.Where(x => x.ActivityId == activityId);
        }
        var sessions = await sessionsQuery
            .OrderBy(x => x.StartDateUtc)
            .ToListAsync();

        // Convert entity to DTO and return
        return sessions.Select(x => _mapper.Map<SessionGetDto>(x));
    }

    /// <inheritdoc/>
    public async Task<SessionGetDto> GetSessionAsync(Guid currUserId, Guid sessionId)
    {
        var session = await _getActiveSession(sessionId);

        // Session doesn't exist
        if (session == null)
        {
            return null;
        }

        // Get the session's activity            
        var activity = await _dbContext.Activities.FirstOrDefaultAsync(x =>
                        x.Id == session.ActivityId &&
                        x.DeletedDateUtc == null);

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

    /// <inheritdoc/>
    public async Task<SessionGetDto> CreateSessionAsync(
        Guid currUserId,
        SessionCreateDto newSessionDto)
    {
        // Get the session's activity            
        var activity = await _dbContext.Activities.FirstOrDefaultAsync(x =>
                        x.Id == newSessionDto.ActivityId &&
                        x.DeletedDateUtc == null);

        // Activity we want to create the session for doesn't exist, return
        if (activity == null)
        {
            return null;
        }

        // Regular users can create their own session. Admins can create sessions for anyone.
        if (currUserId != activity.OwnerId && !(await _userService.IsAdmin(currUserId)))
        {
            throw new ForbiddenException();
        }

        // Convert DTO to entity
        var session =  _mapper.Map<Session>(newSessionDto);

        // Add session
        await _dbContext.Sessions.AddAsync(session);

        // Save to DB
        await _dbContext.SaveChangesAsync();

        return _mapper.Map<SessionGetDto>(session);
    }

    /// <inheritdoc/>
    public async Task<SessionGetDto> UpdateSessionAsync(
        Guid currUserId,
        Guid sessionId,
        SessionUpdateDto updateSessionDto)
    {
        // Get active session with ID
        var session = await _getActiveSession(sessionId);
        
        // Return if activity doesn't exist (or the user was soft deleted)
        if (session == null)
        {
            return null;
        }

        // Get the session's activity            
        var activity = await _dbContext.Activities.FirstOrDefaultAsync(x =>
                        x.Id == session.ActivityId &&
                        x.DeletedDateUtc == null);

        // Regular users can update their own activities. Admins can update anyone's activities.
        if (currUserId != activity.OwnerId && !(await _userService.IsAdmin(currUserId)))
        {
            throw new ForbiddenException();
        }

        // NOTE: You can't change the activity ID associated with a session once it's created
        if (updateSessionDto.Notes != null)
        {
            session.Notes = updateSessionDto.Notes;
        }
        if (updateSessionDto.DurationSeconds != null)
        {
            session.DurationSeconds = (uint) updateSessionDto.DurationSeconds;
        }

        // Save to DB
        await _dbContext.SaveChangesAsync();

        return _mapper.Map<SessionGetDto>(session);
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteSessionAsync(Guid currUserid, Guid sessionId)
    {
        // Get active session with ID
        var session = await _getActiveSession(sessionId);

        // Return if session doesn't exist (or the session was already soft deleted)
        if (session == null)
        {
            return false;
        }

        var activity = await _dbContext.Activities.FirstOrDefaultAsync(x =>
                        x.Id == session.ActivityId &&
                        x.DeletedDateUtc == null);

        if (activity == null)
        {
            return false;
        }

        // Can only delete a session belonging to the user or if admin
        if (currUserid != activity.OwnerId && !(await _userService.IsAdmin(currUserid)))
        {
            throw new ForbiddenException();
        }

        // Soft delete the session
        var utcNow = DateTime.UtcNow;
        // TODO factor doing this out
        session.DeletedDateUtc = new DateTime(utcNow.Year, utcNow.Month, utcNow.Day, utcNow.Hour, utcNow.Minute, utcNow.Second, DateTimeKind.Utc);

        // Save to DB
        // SaveChangesAsync returns the number of entries written to the DB
        await _dbContext.SaveChangesAsync();
        return true;        
    }

    private async Task<Session> _getActiveSession(Guid sessionId)
    {
        return await _dbContext.Sessions
            .FirstOrDefaultAsync(x => x.Id == sessionId && x.DeletedDateUtc == null);
    }
}