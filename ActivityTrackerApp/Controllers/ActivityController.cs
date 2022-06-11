using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ActivityTrackerApp.Constants;
using ActivityTrackerApp.Dtos;
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
        public async Task<ActionResult<IEnumerable<ActivityGetDto>>> GetAllActivitiesForUserAsync([FromBody] Guid userId)
        {
            try
            {
                // -- Verify the user is authenticated --
                var jwtCookie = Request.Cookies[GlobalConstants.JWT_TOKEN_COOKIE_NAME];
                
                if (jwtCookie == null)
                {
                    return Unauthorized("You are not properly authenticated");
                }

                // Verify that the token is still valid
                JwtSecurityToken token;
                try
                {
                    token = jwtService.Verify(jwtCookie);
                }
                catch (Exception e)
                {
                    logger.LogError(e.Message);
                    return Unauthorized("You are not properly authenticated");
                }

                // -- Get current user from token --
                // We verified token above, so this data should be correct
                token.Payload.TryGetValue(ClaimTypes.NameIdentifier, out var currUserId);
                var currUserIdStr = currUserId as string;
                Guid.TryParse(currUserIdStr, out var currUserGuid);

                // -- Try to get all activities  --
                var activities = await _activityService.GetAllActivitiesForUserAsync(currUserGuid, currUserGuid);
                return Ok(activities);
            }
            catch (UnauthorizedAccessException e)
            {
                return Forbid(e.Message);
            }
            catch (Exception e)
            {
                return Problem($"There was a problem getting all the activities for {userId}", statusCode: 500);
            }
        }

        /// <inheritdoc/>
        public async Task<ActionResult<ActivityGetDto>> GetActivityAsync(Guid activityId)
        {
            try
            {
                // -- Verify the user is authenticated --
                var jwtCookie = Request.Cookies[GlobalConstants.JWT_TOKEN_COOKIE_NAME];
                
                if (jwtCookie == null)
                {
                    return Unauthorized("You are not properly authenticated");
                }

                // Verify that the token is still valid
                JwtSecurityToken token;
                try
                {
                    token = jwtService.Verify(jwtCookie);
                }
                catch (Exception e)
                {
                    logger.LogError(e.Message);
                    return Unauthorized("You are not properly authenticated");
                }


                // -- Get current user from token --
                // We verified token above, so this data should be correct
                token.Payload.TryGetValue(ClaimTypes.NameIdentifier, out var currUserId);
                var currUserIdStr = currUserId as string;
                Guid.TryParse(currUserIdStr, out var currUserGuid);

                // -- Try to get --
                var activityGetDto = await _activityService.GetActivityAsync(currUserGuid, activityId);
                return Ok(activityGetDto);
            }
            catch (UnauthorizedAccessException e)
            {
                return Forbid(e.Message);
            }
            catch (Exception e)
            {
                return Problem($"There was a problem getting the activity {activityId}", statusCode: 500);
            }
        }

        /// <inheritdoc/>
        public async Task<ActionResult> CreateActivityAsync(ActivityCreateDto activityPostDto)
        {
            try
            {
                // -- Verify the user is authenticated --
                var jwtCookie = Request.Cookies[GlobalConstants.JWT_TOKEN_COOKIE_NAME];
                
                if (jwtCookie == null)
                {
                    return Unauthorized("You are not properly authenticated");
                }

                // Verify that the token is still valid
                JwtSecurityToken token;
                try
                {
                    token = jwtService.Verify(jwtCookie);
                }
                catch (Exception e)
                {
                    logger.LogError(e.Message);
                    return Unauthorized("You are not properly authenticated");
                }

                // -- Get current user from token --
                // We verified token above, so this data should be correct
                token.Payload.TryGetValue(ClaimTypes.NameIdentifier, out var currUserId);
                var currUserIdStr = currUserId as string;
                Guid.TryParse(currUserIdStr, out var currUserGuid);

                // -- Try to create --
                await _activityService.CreateActivityAsync(currUserGuid, currUserGuid, activityPostDto);
                return Ok(activityPostDto);
            }
            catch (UnauthorizedAccessException e)
            {
                return Forbid(e.Message);
            }
            catch (Exception e)
            {
                return Problem("There was a problem creating the activity", statusCode: 500);
            }
        }

        /// <inheritdoc/>
        public async Task<IActionResult> UpdateActivityAsync(Guid activityId, ActivityUpdateDto activityPutDto)
        {
            try
            {
                // -- Verify the user is authenticated --
                var jwtCookie = Request.Cookies[GlobalConstants.JWT_TOKEN_COOKIE_NAME];
                
                if (jwtCookie == null)
                {
                    return Unauthorized("You are not properly authenticated");
                }

                // Verify that the token is still valid
                JwtSecurityToken token;
                try
                {
                    token = jwtService.Verify(jwtCookie);
                }
                catch (Exception e)
                {
                    logger.LogError(e.Message);
                    return Unauthorized("You are not properly authenticated");
                }

                // -- Get current user from token --
                // We verified token above, so this data should be correct
                token.Payload.TryGetValue(ClaimTypes.NameIdentifier, out var currUserId);
                var currUserIdStr = currUserId as string;
                Guid.TryParse(currUserIdStr, out var currUserGuid);

                // -- Try to update --
                await _activityService.UpdateActivityAsync(currUserGuid, activityId, activityPutDto);
                return Ok(activityPutDto);
            }
            catch (UnauthorizedAccessException e)
            {
                return Forbid(e.Message);
            }
            catch (Exception e)
            {
                return Problem($"There was a problem deleting the activity {activityId}", statusCode: 500);
            }
        }
    

        /// <inheritdoc/>
        public async Task<IActionResult> DeleteActivityAsync(Guid activityId)
        {
            try
            {
                var jwtCookie = Request.Cookies[GlobalConstants.JWT_TOKEN_COOKIE_NAME];
                
                if (jwtCookie == null)
                {
                    return Unauthorized("You are not properly authenticated");
                }

                // Verify that the token is still valid
                JwtSecurityToken token;
                try
                {
                    token = jwtService.Verify(jwtCookie);
                }
                catch (Exception e)
                {
                    logger.LogError(e.Message);
                    return Unauthorized("You are not properly authenticated");
                }

                // -- Get current user from token --
                // We verified token above, so this data should be correct
                token.Payload.TryGetValue(ClaimTypes.NameIdentifier, out var currUserId);
                var currUserIdStr = currUserId as string;
                Guid.TryParse(currUserIdStr, out var currUserGuid);

                // -- Try to delete --
                await _activityService.DeleteActivityAsync(currUserGuid, activityId);
                return Ok("Successfully deleted activity");
            }
            catch (UnauthorizedAccessException e)
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
