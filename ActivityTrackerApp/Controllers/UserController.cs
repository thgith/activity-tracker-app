using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ActivityTrackerApp.Constants;
using ActivityTrackerApp.Dtos;
using ActivityTrackerApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace ActivityTrackerApp.Controllers
{
    /// <summary>
    /// An activity. 
    /// Endpoint will be: api/v1/User
    /// </summary>
    [Route("api/v1/[controller]")]
    public class UserController : ApiControllerBase<UserController>, IUserController
    {
        private readonly IHelperService _helperService;

        public UserController(
            IHelperService helperService,
            IUserService userService,
            IJwtService jwtService,
            ILogger<UserController> logger) : base(userService, jwtService, logger)
        {
            _helperService = helperService ?? throw new ArgumentNullException(nameof(helperService));
        }

        /// <inheritdoc/>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<UserGetDto>>> GetAllUsersAsync()
        {
            try
            {
                // -- Verify the user authenticated --
                // Get JWT info from cookie. Should be stored from login/registration
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
                    return Unauthorized("You are not properly authenticated");
                }
                
                // -- Get current user --
                token.Payload.TryGetValue(ClaimTypes.NameIdentifier, out var currUserId);
                var currUserIdStr = currUserId as string;
                Guid.TryParse(currUserIdStr, out var currUserGuid);

                // -- Get all users --
                var usersDtos = await userService.GetAllUsersAsync(currUserGuid);
                return Ok(usersDtos);
            }
            catch (Exception e)
            {
                var message = $"There was an error getting the users";
                logger.LogError(message, e.Message, e.StackTrace);
                return Problem(message, statusCode: 500);
            }
        }

        /// <inheritdoc/>
        [HttpGet("{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UserGetDto>> GetUserAsync(Guid userId)
        {
            try
            {
                // -- Verify the user is authenticated --
                // Get JWT info from cookie. Should be stored from login/registration
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
                    return Unauthorized("You are not properly authenticated");
                }

                // -- Get current user --
                token.Payload.TryGetValue(ClaimTypes.NameIdentifier, out var currUserId);
                var currUserIdStr = currUserId as string;
                Guid.TryParse(currUserIdStr, out var currUserGuid);

                // -- Get user --
                var user = await userService.GetUserAsync(currUserGuid, userId);
                if (user == null)
                {
                    return NotFound();
                }
                return Ok(user);
            }
            catch (Exception e)
            {
                var message = $"There was an error getting the user with ID {userId}";
                logger.LogError(message, e.Message, e.StackTrace);
                return Problem(message);
            }
        }

        /// <inheritdoc/>
        [HttpPut("{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateUserAsync(
            Guid userId,
            [FromBody] UserUpdateDto userPutDto)
        {
            try
            {
                // TODO separate this out
                // -- Verify the user is authenticated --
                // Get JWT info from cookie. Should be stored from login/registration
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
                    return Unauthorized("You are not properly authenticated");
                }

                // Check email
                if (userPutDto.Email != null && !_helperService.IsEmailValid(userPutDto.Email))
                {
                    return BadRequest("Invalid Email");
                }

                // -- Get current user --
                token.Payload.TryGetValue(ClaimTypes.NameIdentifier, out var currUserId);
                var currUserIdStr = currUserId as string;
                Guid.TryParse(currUserIdStr, out var currUserGuid);

                // -- Update user --
                var user = await userService.UpdateUserAsync(currUserGuid, userId, userPutDto);
                if (user == null)
                {
                    return NotFound($"The user with the ID {userId} does not exist");
                }
                return Ok(user);
            }
            catch (Exception e)
            {
                var message = "There was an error creating the user";
                logger.LogError(message, e.Message, e.StackTrace);
                return Problem(message, statusCode: 500);
            }
        }

        /// <inheritdoc/>
        [HttpDelete("{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteUserAsync(Guid userId)
        {
            try
            {
                // -- Verify the user is authenticated --
                // Get JWT info from cookie. Should be stored from login/registration
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
                    return Unauthorized("You are not properly authenticated");
                }

                // -- Get current user --
                token.Payload.TryGetValue(ClaimTypes.NameIdentifier, out var currentUserId);
                var currentUserIdStr = currentUserId as string;
                Guid.TryParse(currentUserIdStr, out var currUserGuid);

                // -- Delete user --
                var result = await userService.DeleteUserAsync(currUserGuid, userId);
                if (result)
                {
                    return Ok($"Successfully deleted user {userId}");
                }
                return NotFound($"User with ID {userId} does not exist");
            }
            catch (Exception e)
            {
                var message = $"There was a problem deleting the user with ID {userId}";
                logger.LogError(message, e.Message, e.StackTrace);
                return Problem(message, statusCode: 500);
            }
        }
    }
}
