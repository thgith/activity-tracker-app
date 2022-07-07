using ActivityTrackerApp.Database;
using ActivityTrackerApp.Dtos;
using ActivityTrackerApp.Entities;
using ActivityTrackerApp.Exceptions;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace ActivityTrackerApp.Services;

/// <summary>
/// Activity service.
/// </summary>
public class ActivityService : IActivityService
{
    private readonly IDataContext _dbContext;
    private readonly IUserService _userService;
    private readonly ISessionService _sessionService;
    private readonly IMapper _mapper;

    public ActivityService(
        IDataContext dbContext,
        IUserService userService,
        ISessionService sessionService,
        IMapper mapper)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    // <inheritdoc/>
    public async Task<IEnumerable<ActivityGetDto>> GetAllActivitiesForUserAsync(
        Guid currUserId,
        Guid? ownerId = null,
        bool includeSessions = true)
    {
        var isAdmin = await _userService.IsAdmin(currUserId);

        // No owner means get all activities. Only admins can do this.
        if (ownerId == null && !isAdmin)
        {
            throw new ForbiddenException();
        }

        // Regular users can get their own activities. Admins can get everyone's activities.
        if (currUserId != ownerId && !isAdmin)
        {
            throw new ForbiddenException();
        }

        // Get all active activities
        var activitiesQuery = _dbContext.Activities
            .Where(x => x.DeletedDateUtc == null);

        // Add filter on owner if exists
        if (ownerId != null)
        {
            activitiesQuery = activitiesQuery.Where(x => x.OwnerId == ownerId);
        }

        var activities = await activitiesQuery
            .OrderBy(x => x.StartDateUtc)
            .ToListAsync();


        var activitiesGetDtos = activities.Select(x => _mapper.Map<ActivityGetDto>(x)).ToList();
        if (includeSessions)
        {
            foreach (var act in activitiesGetDtos)
            {
                act.Sessions = (await _sessionService.GetAllSessionsByActivityIdAsync(currUserId, act.Id)).ToList();
            }
        }


        // Convert entity to DTO and return
        return activitiesGetDtos;
    }

    // <inheritdoc/>
    public async Task<ActivityGetDto> GetActivityAsync(
        Guid currUserId,
        Guid activityId,
        bool includeSessions = true)
    {
        var activity = await _getActiveActivity(activityId);

        // Activity doesn't exist, return
        if (activity == null)
        {
            return null;
        }

        // Regular users can get their own activities. Admins can get everyone's activities.
        if (currUserId != activity.OwnerId && !(await _userService.IsAdmin(currUserId)))
        {
            throw new ForbiddenException();
        }
        var sessionsForActivity = await _sessionService.GetAllSessionsByActivityIdAsync(currUserId, activity.Id);
        var activityGetDto = _mapper.Map<ActivityGetDto>(activity);
        activityGetDto.Sessions = sessionsForActivity.ToList();

        // Convert entity to DTO and return
        return activityGetDto;
    }

    // <inheritdoc/>
    public async Task<ActivityGetDto> CreateActivityAsync(
        Guid currUserId,
        Guid ownerId,
        ActivityCreateDto newActivityDto)
    {
        // Regular users can create their own activities. Admins can create activities for anyone.
        if (currUserId != ownerId && !(await _userService.IsAdmin(currUserId)))
        {
            throw new ForbiddenException();
        }

        // Convert DTO to entity
        var activity = _mapper.Map<Activity>(newActivityDto);

        // Set the owner of the activity
        activity.OwnerId = ownerId;

        // Autoset as not archived yet
        activity.IsArchived = false;

        // Set default start date if not specified
        DateTime startDateUtc;
        if (newActivityDto.StartDateUtc == null || newActivityDto.StartDateUtc == DateTime.MinValue)
        {
            startDateUtc = DateTime.UtcNow;
        }
        else
        {
            startDateUtc = (DateTime)newActivityDto.StartDateUtc;
        }
        var shortenedStartDateUtc = new DateTime(startDateUtc.Year, startDateUtc.Month, startDateUtc.Day, startDateUtc.Hour, startDateUtc.Minute, startDateUtc.Second, DateTimeKind.Utc);
        activity.StartDateUtc = shortenedStartDateUtc;

        if (newActivityDto.DueDateUtc != null)
        {
            // User set the date to empty
            if (newActivityDto.DueDateUtc == DateTime.MinValue)
            {
                activity.DueDateUtc = null;
            }

            else if (shortenedStartDateUtc > newActivityDto.DueDateUtc)
            {
                throw new InvalidDataException("Start date must occur before due date");
            }
            else
            {
                var dueDateUtc = (DateTime)newActivityDto.DueDateUtc;
                activity.DueDateUtc = new DateTime(dueDateUtc.Year, dueDateUtc.Month, dueDateUtc.Day, dueDateUtc.Hour, dueDateUtc.Minute, dueDateUtc.Second, DateTimeKind.Utc);
            }
        }

        if (newActivityDto.CompletedDateUtc != null)
        {
            // User set the date to empty
            if (newActivityDto.CompletedDateUtc == DateTime.MinValue)
            {
                activity.CompletedDateUtc = null;
            }
            else if (shortenedStartDateUtc > newActivityDto.CompletedDateUtc)
            {
                throw new InvalidDataException("Start date must occur before completed date");
            }
            else
            {
                var completeDateUtc = (DateTime)newActivityDto.CompletedDateUtc;
                activity.CompletedDateUtc = new DateTime(completeDateUtc.Year, completeDateUtc.Month, completeDateUtc.Day, completeDateUtc.Hour, completeDateUtc.Minute, completeDateUtc.Second, DateTimeKind.Utc);
            }
        }

        // Add activity
        await _dbContext.Activities.AddAsync(activity);

        // Save to DB
        await _dbContext.SaveChangesAsync();

        return _mapper.Map<ActivityGetDto>(activity);
    }

    // <inheritdoc/>
    public async Task<ActivityGetDto> UpdateActivityAsync(
        Guid currUserId,
        Guid activityId,
        ActivityUpdateDto updatedActivityDto)
    {
        // Get active activity with ID
        var activity = await _getActiveActivity(activityId);

        // Return if activity doesn't exist (or the user was soft deleted)
        if (activity == null)
        {
            return null;
        }

        // Regular users can update their own activities. Admins can update anyone's activities.
        if (currUserId != activity.OwnerId && !(await _userService.IsAdmin(currUserId)))
        {
            throw new ForbiddenException();
        }

        // Update fields
        if (updatedActivityDto.Name != null)
        {
            activity.Name = updatedActivityDto.Name;
        }

        if (updatedActivityDto.Description != null)
        {
            activity.Description = updatedActivityDto.Description;
        }

        // If start date is set in request, replace; otherwise, use current one stored.
        // Technically, this should be auto set at creation, but just in case set if null
        var startDateUtc = updatedActivityDto.StartDateUtc ?? activity.StartDateUtc ?? DateTime.UtcNow;
        var shortenedStartDateUtc = new DateTime(startDateUtc.Year, startDateUtc.Month, startDateUtc.Day, startDateUtc.Hour, startDateUtc.Minute, startDateUtc.Second, DateTimeKind.Utc);
        activity.startDateUtc = shortenedStartDateUtc;

        // This might still be null if never set
        var dueDateUtc = updatedActivityDto.DueDateUtc ?? activity.DueDateUtc;
        if (dueDateUtc != null)
        {
            // The user set the date to empty
            if (updatedActivityDto.DueDateUtc == DateTime.MinValue)
            {
                activity.DueDateUtc = null;
            }

            else if (shortenedStartDateUtc > dueDateUtc)
            {
                throw new InvalidDataException("Start date must occur before due date");
            }
            else
            {
                var shortenedDueDateUtc = new DateTime(
                    ((DateTime)dueDateUtc).Year,
                    ((DateTime)dueDateUtc).Month,
                    ((DateTime)dueDateUtc).Day,
                    ((DateTime)dueDateUtc).Hour,
                    ((DateTime)dueDateUtc).Minute,
                    ((DateTime)dueDateUtc).Second,
                    DateTimeKind.Utc);
                activity.DueDateUtc = shortenedDueDateUtc;
            }
        }

        // This might still be null if never set
        var completedDateUtc = updatedActivityDto.CompletedDateUtc ?? activity.CompletedDateUtc;
        if (completedDateUtc != null)
        {
            // The user set the date to empty
            if (updatedActivityDto.CompletedDateUtc == DateTime.MinValue)
            {
                activity.CompletedDateUtc = null;
            }

            else if (shortenedStartDateUtc > completedDateUtc)
            {
                throw new InvalidDataException("Start date must occur before completed date");
            }
            else
            {
                var shortenedCompletedDateUtc = new DateTime(
                    ((DateTime)completedDateUtc).Year,
                    ((DateTime)completedDateUtc).Month,
                    ((DateTime)completedDateUtc).Day,
                    ((DateTime)completedDateUtc).Hour,
                    ((DateTime)completedDateUtc).Minute,
                    ((DateTime)completedDateUtc).Second,
                    DateTimeKind.Utc);
                activity.CompletedDateUtc = shortenedCompletedDateUtc;
            }
        }

        if (updatedActivityDto.IsArchived != null)
        {
            activity.IsArchived = (bool)updatedActivityDto.IsArchived;
        }

        if (updatedActivityDto.ColorHex != null)
        {
            activity.ColorHex = updatedActivityDto.ColorHex;
        }

        // TODO process and concat long tags
        if (updatedActivityDto.Tags != null)
        {
            activity.Tags = updatedActivityDto.Tags;
        }

        // Save to DB
        await _dbContext.SaveChangesAsync();

        return _mapper.Map<ActivityGetDto>(activity);
    }

    // <inheritdoc/>
    public async Task<bool> DeleteActivityAsync(
        Guid currUserId,
        Guid activityId)
    {
        // Get active activity with ID
        var activity = await _getActiveActivity(activityId);

        // Return if activity doesn't exist (or the activity was already soft deleted)
        if (activity == null)
        {
            return false;
        }

        // Regular users can delete their own activities. Admins can delete anyone's activities.
        if (currUserId != activity.OwnerId && !(await _userService.IsAdmin(currUserId)))
        {
            throw new ForbiddenException("The user is not authorized to delete this activity");
        }

        // Soft delete the activity
        var utcNow = DateTime.UtcNow;
        var deletedDateUtc = new DateTime(utcNow.Year, utcNow.Month, utcNow.Day, utcNow.Hour, utcNow.Minute, utcNow.Second, DateTimeKind.Utc);
        activity.DeletedDateUtc = deletedDateUtc;

        // Now, delete the sessions associated with the activity
        // Get all of the active sessions associated wtih the activity
        var sessionsToDelete = _dbContext.Sessions.Where(x =>
                                    x.ActivityId == activityId &&
                                    x.DeletedDateUtc == null);

        // Soft delete each session
        foreach (var session in sessionsToDelete)
        {
            session.DeletedDateUtc = deletedDateUtc;
        }

        // Save to DB
        // SaveChangesAsync returns the number of entries written to the DB
        await _dbContext.SaveChangesAsync();
        return true;
    }

    private async Task<Activity> _getActiveActivity(Guid activityId)
    {
        return await _dbContext.Activities
            .FirstOrDefaultAsync(x =>
                x.Id == activityId &&
                x.DeletedDateUtc == null);
    }
}