using ActivityTrackerApp.Dtos;
using ActivityTrackerApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace ActivityTrackerApp.Controllers
{
    /// <summary>
    /// Activity endpoints.
    /// </summary>
    [Route("api/v1/[controller]")]
    public class ActivityController : ApiControllerBase<ActivityController>
    {        
        private readonly IActivityService _activityService;

        public ActivityController(
            IActivityService activityService,
            IUserService userService,
            IJwtService jwtService,
            ILogger<ActivityController> logger) : base(userService, jwtService, logger)
        {
            _activityService = activityService ?? throw new ArgumentNullException(nameof(activityService));
        }

        /// <inheritdoc/>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllActivitiesForUserAsync([FromQuery] Guid userId)
        {
            async Task<IActionResult> GetAllActivitiesForUserPartialAsync(Guid currUserGuid)
            {
                var activities = await _activityService.GetAllActivitiesForUserAsync(currUserGuid, userId);
                return Ok(activities);
            }
            return await checkAuthAndPerformAction(Request, GetAllActivitiesForUserPartialAsync);
        }

        /// <inheritdoc/>
        [HttpGet("{activityId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetActivityAsync(Guid activityId)
        {
            async Task<IActionResult> GetActivityPartialAsync(Guid currUserGuid)
            {
                var activityGetDto = await _activityService.GetActivityAsync(currUserGuid, activityId);
                if (activityGetDto == null)
                {
                    return NotFound($"The activity with the ID {activityId} does not exist");
                }
                return Ok(activityGetDto);
            }
            return await checkAuthAndPerformAction(Request, GetActivityPartialAsync);
        }

        /// <inheritdoc/>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateActivityAsync([FromBody] ActivityCreateDto activityPostDto)
        {
            async Task<IActionResult> CreateActivityPartialAsync(Guid currUserGuid)
            {
                await _activityService.CreateActivityAsync(currUserGuid, currUserGuid, activityPostDto);
                return Ok(activityPostDto);
            }
            return await checkAuthAndPerformAction(Request, CreateActivityPartialAsync);
        }

        /// <inheritdoc/>
        [HttpPut("{activityId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateActivityAsync(
            Guid activityId,
            [FromBody] ActivityUpdateDto activityUpdateDto)
        {
            async Task<IActionResult> UpdateActivityPartialAsync(Guid currUserGuid)
            {
                var activity = await _activityService.UpdateActivityAsync(currUserGuid, activityId, activityUpdateDto);
                if (activity == null)
                {
                    return NotFound($"The activity with the ID {activityId} does not exist");
                }
                return Ok(activityUpdateDto);
            }
            return await checkAuthAndPerformAction(Request, UpdateActivityPartialAsync);
        }

        /// <inheritdoc/>
        [HttpDelete("{activityId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteActivityAsync(Guid activityId)
        {
            async Task<IActionResult> DeleteActivityPartialAsync(Guid currUserGuid)
            {
                var isSuccess = await _activityService.DeleteActivityAsync(currUserGuid, activityId);
                if (!isSuccess)
                {
                    return NotFound($"The activity with the ID {activityId} does not exist");
                }
                return Ok($"Successfully deleted activity {activityId}");
            }
            return await checkAuthAndPerformAction(Request, DeleteActivityPartialAsync);
        }    
    }
}