using System.Linq;
using ActivityTrackerApp.Dtos;
using ActivityTrackerApp.Entities;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace ActivityTrackerApp.Services
{
    /// <summary>
    /// Activity service.
    /// </summary>
    public class ActivityService : IActivityService
    {
        private readonly IDataContext _dbContext;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public ActivityService(
            IDataContext dbContext,
            IUserService userService,
            IMapper mapper)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<ActivityGetDto>> GetAllActivitiesForUserAsync(
            Guid currUserId,
            Guid ownerId)
        {
            // Regular users can get their own activities. Admins can get everyone's activities.
            if (currUserId != ownerId && !(await _userService.IsAdmin(currUserId)))
            {
                throw new UnauthorizedAccessException("The user is not authorized to create an activity for another user");
            }

            var activities = await _dbContext.Activities
                .Where(x => x.OwnerId == ownerId && x.DateDeleted == null)
                .OrderBy(x => x.StartDateUtc)
                .ToListAsync();       
            
            // Convert entity to DTO and return
            return activities.Select(x => _mapper.Map<ActivityGetDto>(x));
        }

        public async Task<ActivityGetDto> GetActivityAsync(
            Guid currUserId,
            Guid activityId)
        {
            var activity = await _getActiveActivity(activityId);

            // Regular users can get their own activities. Admins can get everyone's activities.
            if (currUserId != activity.OwnerId && !(await _userService.IsAdmin(currUserId)))
            {
                throw new UnauthorizedAccessException("The user is not authorized to get an activity of another user");
            }

            if (activity == null)
            {
                return null;
            }

            // Convert entity to DTO and return
            return _mapper.Map<ActivityGetDto>(activity);
        }

        public async Task<ActivityCreateDto> CreateActivityAsync(
            Guid currUserId,
            Guid ownerId,
            ActivityCreateDto newActivityDto)
        {
            // Regular users can create their own activities. Admins can create activities for anyone.
            if (currUserId != ownerId && !(await _userService.IsAdmin(currUserId)))
            {
                throw new UnauthorizedAccessException("The user is not authorized to create an activity for another user");
            }

            // Convert DTO to entity
            var activity =  _mapper.Map<Activity>(newActivityDto);

            // Set the owner of the activity
            activity.OwnerId = ownerId;

            // Add activity
            await _dbContext.Activities.AddAsync(activity);

            // Save to DB
            await _dbContext.SaveChangesAsync();

            return newActivityDto;
        }

        public async Task<ActivityUpdateDto> UpdateActivityAsync(
            Guid currUserId,
            Guid activityId,
            ActivityUpdateDto updatedActivityDto)
        {
            // Get active activity with ID
            var activity = await _getActiveActivity(activityId);
            
            // Regular users can update their own activities. Admins can update anyone's activities.
            if (currUserId != activity.OwnerId && !(await _userService.IsAdmin(currUserId)))
            {
                throw new UnauthorizedAccessException("The user is not authorized to update this activity");
            }

            // Return if activity doesn't exist (or the user was soft deleted)
            if (activity == null)
            {
                return null;
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
            if (updatedActivityDto.DueDateUtc != null)
            {
                activity.DueDateUtc = updatedActivityDto.DueDateUtc;
            }
            if (updatedActivityDto.CompleteDateUtc != null)
            {
                activity.CompleteDateUtc = updatedActivityDto.CompleteDateUtc;
            }
            if (updatedActivityDto.ArchiveDateUtc != null)
            {
                activity.ArchiveDateUtc = updatedActivityDto.ArchiveDateUtc;
            }
            if (updatedActivityDto.ColorHex != null)
            {
                activity.ColorHex = updatedActivityDto.ColorHex;
            }
            // TODO process and concat long tags
            // TODO prob change Tags to List
            if (updatedActivityDto.Tags != null)
            {
                activity.Tags = updatedActivityDto.Tags;
            }

            // Save to DB
            await _dbContext.SaveChangesAsync();

            return updatedActivityDto;
        }

        public async Task<bool> DeleteActivityAsync(
            Guid currUserId,
            Guid activityId)
        {
            // Get active activity with ID
            var activity = await _getActiveActivity(activityId);

            // Regular users can delete their own activities. Admins can delete anyone's activities.
            if (currUserId != activity.OwnerId && !(await _userService.IsAdmin(currUserId)))
            {
                throw new UnauthorizedAccessException("The user is not authorized to delete this activity");
            }

            // Return if session doesn't exist (or the session was already soft deleted)
            if (activityId == null)
            {
                return false;
            }

            // Soft delete the activity
            activity.DateDeleted = DateTime.UtcNow;

            // Now, delete the sessions associated with the activity
            if (activity.Sessions != null && activity.Sessions.Count() > 0)
            {
                // Get all of the active sessions in the provided list
                var sessionsToDelete = _dbContext.Sessions.Where(x => 
                                            activity.Sessions.Select(x => x.Id)
                                            .Contains(x.Id) && 
                                            x.DateDeleted == null);

                // Soft delete each session
                foreach (var session in sessionsToDelete)
                {
                    session.DateDeleted = DateTime.UtcNow;
                }
            }

            // Save to DB
            // SaveChangesAsync returns the number of entries written to the DB
            await _dbContext.SaveChangesAsync();
            return true;
        }

        private async Task<Activity> _getActiveActivity(Guid activityId)
        {
            return await _dbContext.Activities
                .FirstOrDefaultAsync(x => x.Id == activityId && x.DateDeleted == null);
        }
    }
}