using ActivityTrackerApp.Dtos;

namespace ActivityTrackerApp.Services
{
    /// <summary>
    /// Activity service.
    /// </summary>
    public interface IActivityService
    {
        Task<IEnumerable<ActivityGetDto>> GetAllActivitiesForUserAsync(
            Guid currUserId,
            Guid ownerId);

        Task<ActivityGetDto> GetActivityAsync(
            Guid currUserId,
            Guid activityId);

        Task<ActivityCreateDto> CreateActivityAsync(
            Guid currUserId,
            Guid ownerId,
            ActivityCreateDto newActivityDto);

        Task<ActivityUpdateDto> UpdateActivityAsync(
            Guid currUserId,
            Guid activityId,
            ActivityUpdateDto updatedActivityDto);

        Task<bool> DeleteActivityAsync(
            Guid currUserid,
            Guid activityId);      
    }
}