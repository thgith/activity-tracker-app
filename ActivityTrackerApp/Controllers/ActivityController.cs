using System.Security.Claims;
using ActivityTrackerApp.Constants;
using ActivityTrackerApp.Dtos;
using ActivityTrackerApp.Exceptions;
using ActivityTrackerApp.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ActivityTrackerApp.Controllers
{
    /// <summary>
    /// Activity endpoints.
    /// </summary>
    [Route("api/v1/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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
        public async Task<ActionResult<IEnumerable<ActivityGetDto>>> GetAllActivitiesForUserAsync([FromBody] Guid userId)
        {
            try
            {
                // -- Verify the user is authenticated --
                var jwtCookie = Request.Cookies[GlobalConstants.JWT_TOKEN_COOKIE_NAME];
                var token = jwtService.CheckAuthenticated(jwtCookie);

                // -- Get current user from token --
                // We verified token above, so this data should be correct
                token.Payload.TryGetValue(ClaimTypes.NameIdentifier, out var currUserId);
                var currUserIdStr = currUserId as string;
                Guid.TryParse(currUserIdStr, out var currUserGuid);

                // -- Try to get all activities  --
                var activities = await _activityService.GetAllActivitiesForUserAsync(currUserGuid, currUserGuid);
                return Ok(activities);
            }
            catch (UnauthenticatedException e)
            {
                return Unauthorized(e.Message);
            }
            catch (ForbiddenException e)
            {
                return Forbid(e.Message);
            }
            catch (Exception e)
            {
                return Problem($"There was a problem getting all the activities for {userId}", statusCode: 500);
            }
        }

        /// <inheritdoc/>
        [HttpGet("{activityId}")]
        public async Task<ActionResult<ActivityGetDto>> GetActivityAsync(Guid activityId)
        {
            try
            {
                // -- Verify the user is authenticated --
                var jwtCookie = Request.Cookies[GlobalConstants.JWT_TOKEN_COOKIE_NAME];
                var token = jwtService.CheckAuthenticated(jwtCookie);

                // -- Get current user from token --
                // We verified token above, so this data should be correct
                token.Payload.TryGetValue(ClaimTypes.NameIdentifier, out var currUserId);
                var currUserIdStr = currUserId as string;
                Guid.TryParse(currUserIdStr, out var currUserGuid);

                // -- Try to get --
                var activityGetDto = await _activityService.GetActivityAsync(currUserGuid, activityId);
                return Ok(activityGetDto);
            }
            catch (UnauthenticatedException e)
            {
                return Unauthorized(e.Message);
            }
            catch (ForbiddenException e)
            {
                return Forbid(e.Message);
            }
            catch (Exception e)
            {
                return Problem($"There was a problem getting the activity {activityId}", statusCode: 500);
            }
        }

        /// <inheritdoc/>
        [HttpPost]
        public async Task<ActionResult> CreateActivityAsync([FromBody] ActivityCreateDto activityPostDto)
        {
            try
            {
                // -- Verify the user is authenticated --
                var jwtCookie = Request.Cookies[GlobalConstants.JWT_TOKEN_COOKIE_NAME];
                var token = jwtService.CheckAuthenticated(jwtCookie);

                // -- Get current user from token --
                // We verified token above, so this data should be correct
                token.Payload.TryGetValue(ClaimTypes.NameIdentifier, out var currUserId);
                var currUserIdStr = currUserId as string;
                Guid.TryParse(currUserIdStr, out var currUserGuid);

                // -- Try to create --
                await _activityService.CreateActivityAsync(currUserGuid, currUserGuid, activityPostDto);
                return Ok(activityPostDto);
            }
            catch (UnauthenticatedException e)
            {
                return Unauthorized(e.Message);
            }
            catch (ForbiddenException e)
            {
                return Forbid(e.Message);
            }
            catch (Exception e)
            {
                return Problem("There was a problem creating the activity", statusCode: 500);
            }
        }

        /// <inheritdoc/>
        [HttpPut("{activityId}")]
        public async Task<IActionResult> UpdateActivityAsync(
            Guid activityId,
            [FromBody] ActivityUpdateDto activityPutDto)
        {
            try
            {
                // -- Verify the user is authenticated --
                var jwtCookie = Request.Cookies[GlobalConstants.JWT_TOKEN_COOKIE_NAME];
                var token = jwtService.CheckAuthenticated(jwtCookie);

                // -- Get current user from token --
                // We verified token above, so this data should be correct
                token.Payload.TryGetValue(ClaimTypes.NameIdentifier, out var currUserId);
                var currUserIdStr = currUserId as string;
                Guid.TryParse(currUserIdStr, out var currUserGuid);

                // -- Try to update --
                await _activityService.UpdateActivityAsync(currUserGuid, activityId, activityPutDto);
                return Ok(activityPutDto);
            }
            catch (UnauthenticatedException e)
            {
                return Unauthorized(e.Message);
            }
            catch (ForbiddenException e)
            {
                return Forbid(e.Message);
            }
            catch (Exception e)
            {
                return Problem($"There was a problem deleting the activity {activityId}", statusCode: 500);
            }
        }

        /// <inheritdoc/>
        [HttpDelete("{activityId}")]
        public async Task<IActionResult> DeleteActivityAsync(Guid activityId)
        {
            try
            {
                // -- Verify the user is authenticated --
                var jwtCookie = Request.Cookies[GlobalConstants.JWT_TOKEN_COOKIE_NAME];
                var token = jwtService.CheckAuthenticated(jwtCookie);                

                // -- Get current user from token --
                // We verified token above, so this data should be correct
                token.Payload.TryGetValue(ClaimTypes.NameIdentifier, out var currUserId);
                var currUserIdStr = currUserId as string;
                Guid.TryParse(currUserIdStr, out var currUserGuid);

                // -- Try to delete --
                await _activityService.DeleteActivityAsync(currUserGuid, activityId);
                return Ok("Successfully deleted activity");
            }
            catch (UnauthenticatedException e)
            {
                return Unauthorized(e.Message);
            }
            catch (ForbiddenException e)
            {
                return Forbid(e.Message);
            }
            catch (Exception e)
            {
                return Problem($"There was a problem deleting the activity {activityId}", statusCode: 500);
            }
        }    
    }
}
